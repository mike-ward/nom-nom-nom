using System;
using System.Collections.Generic;
using System.Linq;
using nom_nom_nom.Infrastructure;
using nom_nom_nom.Models;

namespace nom_nom_nom.Services
{
    public class Report : IReport
    {
        private readonly IDataFile _dataFile;

        public Report(IDataFile dataFile)
        {
            _dataFile = dataFile;
        }

        public string Summary()
        {
            const int indent = 12;
            string pad = new string(' ', indent);
            string separator = $"{pad}-------------------";
            var meals = _dataFile.Meals();
            var foods = _dataFile.Foods();
            var groups = meals.GroupBy(tmm => tmm.Time.DayOfYear).OrderBy(group => group.Key);
            var summary = new List<string>();

            foreach (var group in groups)
            {
                summary.Add(group.Select(m => $"{m.Time:ddd MMM dd}").First());
                summary.AddRange(group.Select(m => $"{m.MealType.PadRight(indent)}{string.Join(", ", m.Menu)}"));

                var cost = group
                    .Select(m => ComputeMealCost(m, foods))
                    .Aggregate(new Food(), (a, c) =>
                    {
                        a.Calories += c.Calories;
                        a.Protein += c.Protein;
                        a.Fat += c.Fat;
                        a.Carbs += c.Carbs;
                        return a;
                    });

                summary.Add(separator);
                summary.Add($"{pad}calories={cost.Calories:N0} protein={cost.Protein:N1} fat={cost.Fat:N1} carbs={cost.Carbs:N1}");
                summary.Add("");
            }

            var total = ComputeAllMealsCost(meals, foods);
            summary.Add($"{pad}===================");
            var totalGrams = total.Protein + total.Fat + total.Carbs;
            var percentProtein = total.Protein / totalGrams * 100;
            var percentFat = total.Fat / totalGrams * 100;
            var percentCarbs = total.Carbs / totalGrams * 100;
            summary.Add($"{pad}calories={total.Calories:N0} " +
                        $"protein={total.Protein:N1} (%{percentProtein:N1}) " +
                        $"fat={total.Fat:N1} (%{percentFat:N1}) " +
                        $"carbs={total.Carbs:N1} (%{percentCarbs:N1})");

            var untracked = Untracked(meals, foods);
            if (untracked.Any())
            {
                summary.Add($"{Environment.NewLine}Untracked:");
                summary.AddRange(untracked.Select(ut => $"\t{ut}"));
            }

            return string.Join(Environment.NewLine, summary);
        }

        private static Food ComputeMealCost(Meal meal, IReadOnlyDictionary<string, Food> foods)
        {
            var cost = meal.Menu.Aggregate(new Food(), (a, c) => foods.TryGetValue(c, out var food) ? a + food : a);
            return cost;
        }

        private static Food ComputeAllMealsCost(IEnumerable<Meal> meals, IReadOnlyDictionary<string, Food> foods)
        {
            var total = meals
                .Select(meal => ComputeMealCost(meal, foods))
                .Aggregate(new Food(), (a, c) => a + c);

            return total;
        }

        private static string[] Untracked(IEnumerable<Meal> meals, IReadOnlyDictionary<string, Food> foods)
        {
            var untracked = meals
                .SelectMany(meal => meal.Menu)
                .Where(m => foods.ContainsKey(m) == false)
                .Distinct()
                .OrderBy(o => o)
                .ToArray();
            return untracked;
        }
    }
}

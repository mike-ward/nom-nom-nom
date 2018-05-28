using System;
using System.Collections.Generic;
using System.Linq;
using nom_nom_nom.Comparers;
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
            var pad = new string(' ', indent);
            var separator = $"{pad}-------------------";

            string Format(Food food) => $"{pad}calories {food.Calories:N0} " +
                                        $"protein {food.Protein:N1} (%{food.PercentProtein:N1}) " +
                                        $"fat {food.Fat:N1} (%{food.PercentFat:N1}) " +
                                        $"carbs {food.Carbs:N1} (%{food.PercentCarbs:N1})";

            var meals = _dataFile.Meals();
            var foods = _dataFile.Foods();

            var groups = meals
                .OrderBy(meal => meal.MealType, new OrderedComparer(new[] { Meal.Breakfast, Meal.Lunch, Meal.Dinner, Meal.Snack }))
                .GroupBy(tmm => tmm.Time.DayOfYear)
                .Where(tmm => new[] { Meal.Breakfast, Meal.Lunch, Meal.Dinner}.All(mt => tmm.Any(tm => tm.MealType.Contains(mt))))
                .OrderBy(group => group.Key)
                .ToList();

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
                summary.Add(Format(cost));
                summary.Add("");
            }

            var total = ComputeAllMealsCost(meals, foods);
            var days = groups.Count;

            summary.Add($"{pad}total ({days} days)");
            summary.Add($"{pad}===================");
            summary.Add(Format(total));

            summary.Add("");
            summary.Add($"{pad}per day averages");
            summary.Add($"{pad}===================");
            var average = total / days;
            summary.Add(Format(average));

            var tracked = Tracked(meals, foods);
            if (tracked.Any())
            {
                summary.Add($"{Environment.NewLine}Foods:");
                summary.AddRange(tracked.Select(ut => $"\t{ut}"));
            }

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

        private static string[] Tracked(IEnumerable<Meal> meals, IReadOnlyDictionary<string, Food> foods)
        {
            var tracked = meals
                .SelectMany(meal => meal.Menu)
                .Where(foods.ContainsKey)
                .Distinct()
                .OrderBy(o => o);

            var trackedFoods = foods
                .Where(food => tracked.Any(t => t == food.Key))
                .Select(food => $"{food.Key,-28}  {food.Value.Calories,10:N1} {food.Value.Protein,10:N1} {food.Value.Fat,10:N1} {food.Value.Carbs,10:N1}")
                .ToArray();

            const string header = "                                Calories    Protein        Fat      Carbs";
            return new []{header}.Concat(trackedFoods).ToArray();
        }
    }
}

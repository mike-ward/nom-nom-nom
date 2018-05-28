using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using nom_nom_nom.Infrastructure;
using nom_nom_nom.Models;

namespace nom_nom_nom.Services
{
    public class DataFile : IDataFile
    {
        public List<Meal> Meals()
        {
            var lines = File.ReadAllLines("C:\\Users\\mike\\Google Drive\\notes.txt");
            var mealTypes = new[] { Meal.Breakfast, Meal.Lunch, Meal.Dinner, Meal.Snack };
            var meals = lines.Where(item => mealTypes.Any(mt => item.IndexOf(mt, StringComparison.CurrentCultureIgnoreCase) >= 0));

            var tmms = meals
                .Select(meal => meal.Split(',', 3))
                .Select(mmm => new Meal
                {
                    Time = DateTime.TryParse(mmm[0], out var time) ? time : DateTime.MinValue,
                    MealType = mmm[1].Trim(),
                    Menu = mmm[2].Split(',').Select(m => m.Trim().ToLowerInvariant()).ToArray()
                })
                .ToList();

            return tmms;
        }

        public Dictionary<string, Food> Foods()
        {
            var text = File.ReadAllLines("C:\\Users\\mike\\Google Drive\\food.txt");
            var lines = text.Where(t => !t.StartsWith("//"));

            var foods = lines
                .Select(line => line.Split(','))
                .Where(s => s.Length == 5)
                .Select(s => new Food
                {
                    Name = s[0].Trim(),
                    Calories = ToDouble(s[1]),
                    Protein = ToDouble(s[2]),
                    Fat = ToDouble(s[3]),
                    Carbs = ToDouble(s[4])

                })
                .ToDictionary(k => k.Name, v => v);

            return foods;
        }

    private static double ToDouble(string str)
    {
        return Convert.ToDouble(str);
    }
}
}

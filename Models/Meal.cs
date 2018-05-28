using System;

namespace nom_nom_nom.Models
{
    public class Meal
    {
        public const string Breakfast = "breakfast";
        public const string Lunch = "lunch";
        public const string Dinner = "dinner";
        public const string Snack = "snack";

        public DateTime Time { get; set; }
        public string  MealType { get; set; }
        public string[] Menu { get; set; }
    }
}

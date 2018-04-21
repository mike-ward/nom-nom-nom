using System;

namespace nom_nom_nom.Models
{
    public class Meal
    {
        public DateTime Time { get; set; }
        public string  MealType { get; set; }
        public string[] Menu { get; set; }
    }
}

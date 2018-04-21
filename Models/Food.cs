namespace nom_nom_nom.Models
{
    public class Food
    {
        public string Name { get; set; }
        public double Calories { get; set; }
        public double Protein { get; set; }
        public double Fat { get; set; }
        public double Carbs { get; set; }

        private double Nutrients => Protein + Fat + Carbs;
        public double PercentProtein => Protein / Nutrients * 100; 
        public double PercentFat => Fat / Nutrients * 100; 
        public double PercentCarbs => Carbs / Nutrients * 100; 

        public static Food operator +(Food a, Food b)
        {
            var sum = new Food
            {
                Name = a.Name == b.Name ? a.Name : string.Empty,
                Calories = a.Calories + b.Calories,
                Protein = a.Protein + b.Protein,
                Fat = a.Fat + b.Fat,
                Carbs = a.Carbs + b.Carbs
            };
            return sum;
        }

        public static Food operator /(Food a, int days)
        {
            var food = new Food
            {
                Name = a.Name,
                Calories = a.Calories / days,
                Protein = a.Protein / days,
                Fat = a.Fat / days,
                Carbs = a.Carbs / days
            };
            return food;
        }
    }
}

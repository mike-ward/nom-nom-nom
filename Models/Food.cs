namespace nom_nom_nom.Models
{
    public class Food
    {
        public string Name { get; set; }
        public double Calories { get; set; }
        public double Protein { get; set; }
        public double Fat { get; set; }
        public double Carbs { get; set; }

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
    }
}

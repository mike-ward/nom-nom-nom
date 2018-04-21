using System.Collections.Generic;
using nom_nom_nom.Models;

namespace nom_nom_nom.Infrastructure
{
    public interface IDataFile
    {
        List<Meal> Meals();
        Dictionary<string, Food> Foods();
    }
}
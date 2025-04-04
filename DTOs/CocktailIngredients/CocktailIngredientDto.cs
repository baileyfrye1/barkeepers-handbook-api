using api.DTOs.Ingredients;
using api.Models;

namespace api.DTOs.CocktailIngredients
{
    public class CocktailIngredientDto
    {
        public IngredientDto Ingredient { get; set; } = new();

        public double? Amount { get; set; }

        public string Unit { get; set; } = string.Empty;

    }
}
using api.DTOs.IngredientDTOs;

namespace api.DTOs.CocktailIngredientDTOs;

public class CocktailIngredientDto
{
    public IngredientDto Ingredient { get; set; } = new();

    public double? Amount { get; set; }

    public string Unit { get; set; } = string.Empty;

}
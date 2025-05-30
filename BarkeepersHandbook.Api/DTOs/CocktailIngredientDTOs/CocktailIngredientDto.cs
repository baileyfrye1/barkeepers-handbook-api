using BarkeepersHandbook.Api.DTOs.IngredientDTOs;

namespace BarkeepersHandbook.Api.DTOs.CocktailIngredientDTOs;

public class CocktailIngredientDto
{
    public IngredientDto Ingredient { get; set; } = new();

    public double? Amount { get; set; }

    public string Unit { get; set; } = string.Empty;

}
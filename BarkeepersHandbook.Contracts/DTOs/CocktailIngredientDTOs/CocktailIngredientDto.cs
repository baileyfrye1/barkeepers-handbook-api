using BarkeepersHandbook.Contracts.DTOs.IngredientDTOs;

namespace BarkeepersHandbook.Contracts.DTOs.CocktailIngredientDTOs;

public class CocktailIngredientDto
{
    public IngredientDto Ingredient { get; set; } = new();

    public double? Amount { get; set; }

    public string Unit { get; set; } = string.Empty;

}
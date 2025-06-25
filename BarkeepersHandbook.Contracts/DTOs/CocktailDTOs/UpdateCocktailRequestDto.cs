using BarkeepersHandbook.Contracts.DTOs.CocktailIngredientDTOs;

namespace BarkeepersHandbook.Contracts.DTOs.CocktailDTOs;

public class UpdateCocktailRequestDto
{
	public string? Name { get; set; } = string.Empty;

	public bool? Featured { get; set; }

	public List<string>? Tags { get; set; } = [];

	public List<CocktailIngredientDto>? CocktailIngredients { get; init; } = [];
}
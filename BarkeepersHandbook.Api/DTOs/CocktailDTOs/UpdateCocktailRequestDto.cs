using BarkeepersHandbook.Api.Models;

namespace BarkeepersHandbook.Api.DTOs.CocktailDTOs;

public class UpdateCocktailRequestDto
{
	public string? Name { get; set; } = string.Empty;

	public bool? Featured { get; set; }

	public List<string>? Tags { get; set; } = [];

	public List<CocktailIngredient>? CocktailIngredients { get; set; } = [];
}
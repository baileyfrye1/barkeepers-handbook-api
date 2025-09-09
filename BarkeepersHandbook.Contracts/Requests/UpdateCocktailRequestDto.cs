using BarkeepersHandbook.Contracts.DTOs.CocktailIngredientDTOs;

namespace BarkeepersHandbook.Contracts.Requests;

public class UpdateCocktailRequestDto
{
    public string? Name { get; set; }
    public bool? Featured { get; set; }
    public HashSet<string>? Tags { get; set; }
    public List<CocktailIngredientDto> CocktailIngredients { get; set; } = [];
}
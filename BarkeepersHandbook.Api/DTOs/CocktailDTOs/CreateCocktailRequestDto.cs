using System.ComponentModel.DataAnnotations;
using BarkeepersHandbook.Api.DTOs.CocktailIngredientDTOs;

namespace BarkeepersHandbook.Api.DTOs.CocktailDTOs;

public class CreateCocktailRequestDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    public bool Featured { get; set; }

    public List<string> Tags { get; set; } = [];

    [Required]
    public List<CocktailIngredientDto>? CocktailIngredients { get; set; } = [];

    public IFormFile Image { get; set; }

    // public List<string> Steps { get; set; }
}
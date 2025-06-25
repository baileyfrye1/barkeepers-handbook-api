using System.ComponentModel.DataAnnotations;
using BarkeepersHandbook.Contracts.DTOs.CocktailIngredientDTOs;
using Microsoft.AspNetCore.Http;

namespace BarkeepersHandbook.Contracts.DTOs.CocktailDTOs;

public class CreateCocktailRequestDto
{
    public string Name { get; init; } = string.Empty;

    public bool Featured { get; init; }

    public List<string> Tags { get; init; } = [];

    public List<CocktailIngredientDto>? CocktailIngredients { get; init; } = [];

    public IFormFile Image { get; init; }

    // public List<string> Steps { get; init; }
}
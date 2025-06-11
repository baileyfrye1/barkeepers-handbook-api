using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BarkeepersHandbook.Application.DTOs.CocktailIngredientDTOs;
using Microsoft.AspNetCore.Http;

namespace BarkeepersHandbook.Application.DTOs.CocktailDTOs;

public class CreateCocktailRequestDto
{
    [Required]
    public string Name { get; init; } = string.Empty;

    public bool Featured { get; init; }

    public List<string> Tags { get; init; } = [];

    [Required]
    public List<CocktailIngredientDto>? CocktailIngredients { get; init; } = [];

    public IFormFile Image { get; init; }

    // public List<string> Steps { get; init; }
}
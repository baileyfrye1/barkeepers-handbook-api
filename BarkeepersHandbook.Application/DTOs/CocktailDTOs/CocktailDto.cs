using BarkeepersHandbook.Application.DTOs.CocktailIngredientDTOs;
using BarkeepersHandbook.Application.DTOs.RatingDTOs;

namespace BarkeepersHandbook.Application.DTOs.CocktailDTOs;

public class CocktailDto
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public bool Featured { get; init; }

    public List<string> Tags { get; init; } = [];

    public string UserId { get; init; } = string.Empty;

    public List<CocktailIngredientDto> CocktailIngredients { get; init; } = [];
    
    public string ImageUrl { get; init; } = string.Empty;
    
    public CocktailRatingObjectDto RatingsData { get; init; } = new();

    public DateTime CreatedAt { get; init; }

    public DateTime UpdatedAt { get; init; }
}
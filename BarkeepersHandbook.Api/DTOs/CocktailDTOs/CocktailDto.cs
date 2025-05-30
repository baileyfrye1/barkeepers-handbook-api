using BarkeepersHandbook.Api.DTOs.CocktailIngredientDTOs;
using BarkeepersHandbook.Api.DTOs.RatingDTOs;

namespace BarkeepersHandbook.Api.DTOs.CocktailDTOs;

public class CocktailDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool Featured { get; set; }

    public List<string> Tags { get; set; } = [];

    public string UserId { get; set; } = string.Empty;

    public List<CocktailIngredientDto> CocktailIngredients { get; set; } = [];
    
    public string ImageUrl { get; set; } = string.Empty;
    
    public CocktailRatingObjectDto RatingsData { get; set; } = new();

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
using api.DTOs.CocktailIngredientDTOs;
using api.DTOs.RatingDTOs;

namespace api.DTOs.CocktailDTOs;

public class CocktailDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public bool Featured { get; set; }

    public List<string> Tags { get; set; } = [];

    public List<CocktailIngredientDto> CocktailIngredients { get; set; } = [];
    
    public CocktailRatingObjectDto RatingsData { get; set; } = new();

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
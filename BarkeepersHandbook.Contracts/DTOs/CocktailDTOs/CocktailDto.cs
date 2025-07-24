using BarkeepersHandbook.Contracts.DTOs.CocktailIngredientDTOs;
using BarkeepersHandbook.Contracts.DTOs.RatingDTOs;

namespace BarkeepersHandbook.Contracts.DTOs.CocktailDTOs;
public record CocktailDto(int Id, string Name, bool Featured, List<string> Tags, string UserId, List<CocktailIngredientDto> CocktailIngredients, string ImageUrl, CocktailRatingsOverviewDto RatingsData, DateTime CreatedAt, DateTime UpdatedAt);
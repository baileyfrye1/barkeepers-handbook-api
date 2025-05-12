using api.DTOs.CocktailDTOs;

namespace api.DTOs.RatingDTOs;

public class RatingDto
{
   public int Id { get; set; } 
   public int Rating { get; set; }
   public RatedCocktailDto Cocktail { get; set; } = new RatedCocktailDto();
   public string UserId { get; set; } = String.Empty;
}
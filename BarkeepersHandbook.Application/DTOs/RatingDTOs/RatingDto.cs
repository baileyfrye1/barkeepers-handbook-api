using BarkeepersHandbook.Application.DTOs.CocktailDTOs;

namespace BarkeepersHandbook.Application.DTOs.RatingDTOs;

public class RatingDto
{
   public int Id { get; set; } 
   public int Rating { get; set; }
   public ReferenceCocktailDto Cocktail { get; set; } = new ReferenceCocktailDto();
   public string UserId { get; set; } = string.Empty;
}
using BarkeepersHandbook.Api.DTOs.CocktailDTOs;

namespace BarkeepersHandbook.Api.DTOs.FavoriteDTOs;

public class FavoriteDto
{
    public int Id { get; set; }

    public ReferenceCocktailDto Cocktail { get; set; } = new ReferenceCocktailDto() { };
    
    public string UserId { get; set; } = string.Empty;
}
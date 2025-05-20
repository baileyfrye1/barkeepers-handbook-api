using api.DTOs.CocktailDTOs;

namespace api.DTOs.FavoriteDTOs;

public class FavoriteDto
{
    public int Id { get; set; }

    public ReferenceCocktailDto Cocktail { get; set; } = new ReferenceCocktailDto() { };
    
    public string UserId { get; set; } = string.Empty;
}
using BarkeepersHandbook.Contracts.DTOs.CocktailDTOs;

namespace BarkeepersHandbook.Contracts.DTOs.FavoriteDTOs;

public class FavoriteDto
{
    public int Id { get; set; }

    public ReferenceCocktailDto Cocktail { get; set; } = new ReferenceCocktailDto() { };
    
    public string UserId { get; set; } = string.Empty;
}
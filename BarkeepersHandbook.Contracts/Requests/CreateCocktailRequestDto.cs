using BarkeepersHandbook.Contracts.DTOs.CocktailIngredientDTOs;
using Microsoft.AspNetCore.Http;

namespace BarkeepersHandbook.Contracts.Requests;

public record CreateCocktailRequestDto(
    string Name,
    bool Featured, 
    HashSet<string>? Tags,
    List<CocktailIngredientDto> CocktailIngredients, 
    IFormFile? Image
);
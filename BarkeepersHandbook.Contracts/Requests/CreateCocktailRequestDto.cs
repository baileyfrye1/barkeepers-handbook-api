using BarkeepersHandbook.Contracts.DTOs.CocktailIngredientDTOs;
using Microsoft.AspNetCore.Http;

namespace BarkeepersHandbook.Contracts.Requests;

public record CreateCocktailRequestDto(
    string Name,
    bool Featured, 
    List<string>? Tags,
    List<CocktailIngredientDto> CocktailIngredients, 
    IFormFile? Image
);
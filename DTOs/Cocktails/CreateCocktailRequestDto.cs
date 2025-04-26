using System.ComponentModel.DataAnnotations;
using api.DTOs.CocktailIngredients;

namespace api.DTOs.Cocktails
{
    public class CreateCocktailRequestDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public bool Featured { get; set; }

        public List<string> Tags { get; set; } = [];

        [Required]
        public List<CocktailIngredientDto> CocktailIngredients { get; set; } = [];

        // public IFormFile Image { get; set; }

        // public List<string> Steps { get; set; }
    }
}
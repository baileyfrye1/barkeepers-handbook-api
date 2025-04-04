using api.DTOs.CocktailIngredients;

namespace api.DTOs.Cocktails
{
    public class CocktailDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public bool Featured { get; set; }

        public List<string> Tags { get; set; } = [];

        public List<CocktailIngredientDto> CocktailIngredients { get; set; } = [];

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}

using api.DTOs;
using api.DTOs.RatingDTOs;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace api.Models
{
    [Table("cocktails")]
    public class Cocktail : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("featured")]
        public bool Featured { get; set; }

        [Column("tags")]
        public List<string> Tags { get; set; } = [];

        [Column("image")]
        public string ImageUrl { get; set; }

        [Column("steps")]
        public List<string>? Steps { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [Reference(typeof(CocktailIngredient))]
        public List<CocktailIngredient> CocktailIngredients { get; set; } = [];
    }
}
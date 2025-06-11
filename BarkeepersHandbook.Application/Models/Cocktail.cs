using System;
using System.Collections.Generic;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace BarkeepersHandbook.Application.Models
{
    [Table("cocktails")]
    public class Cocktail : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; init; }

        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("featured")]
        public bool Featured { get; set; }

        [Column("user_id")]
        public string UserId { get; set; }

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

        [Reference(typeof(Rating), useInnerJoin: false)]
        public List<Rating> Ratings { get; set; }
    }
}

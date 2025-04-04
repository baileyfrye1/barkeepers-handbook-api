using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace api.Models
{
	[Table("favorites")]
	public class Favorite : BaseModel
	{
		[PrimaryKey("id", false)]
		public int Id { get; set; }

		[Reference(typeof(Cocktail))]
		public Cocktail Cocktail { get; set; } = new Cocktail { };

		[Column("cocktail_id")]
		public int CocktailId { get; set; }

		[Column("created_at")]
		public DateTime CreatedAt { get; set; }

		[Column("updated_at")]
		public DateTime UpdatedAt { get; set; }

		public Favorite() { }
	}
}
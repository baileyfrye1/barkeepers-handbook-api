using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace api.Models
{

	[Table("reviews")]
	public class Review : BaseModel
	{
		[PrimaryKey("id")]
		public int Id { get; set; }

		[Reference(typeof(Cocktail))]
		public Cocktail Cocktail { get; set; } = new Cocktail { };

		[Column("cocktail_id")]
		public int CocktailId { get; set; }

		[Column("rating")]
		public int Rating { get; set; }

		[Column("user_id")]
		public string UserId { get; set; } = string.Empty;
		
		[Column("created_at")]
		public DateTime CreatedAt { get; set; }

		[Column("updated_at")]
		public DateTime UpdatedAt { get; set; }
	}
}
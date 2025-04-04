using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace api.Models
{

	[Table("reviews")]
	public class Review : BaseModel
	{
		[PrimaryKey("id", false)]
		public int Id { get; set; }

		[Reference(typeof(Cocktail))]
		public Cocktail Cocktail { get; set; } = new Cocktail { };

		[Column("cocktailId")]
		public int CocktailId { get; set; }

		[Column("rating")]
		public int Rating { get; set; }

		[Column("comment")]
		public string Comment { get; set; } = string.Empty;

		[Column("authorName")]
		public string AuthorName { get; set; } = string.Empty;

		[Column("authorImageUrl")]
		public string AuthorImageUrl { get; set; } = string.Empty;

		[Column("createdAt")]
		public DateTime CreatedAt { get; set; }

		[Column("updatedAt")]
		public DateTime UpdatedAt { get; set; }

		public Review() { }
	}
}
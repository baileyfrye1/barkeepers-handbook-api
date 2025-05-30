using BarkeepersHandbook.Api.DTOs.CocktailDTOs;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace BarkeepersHandbook.Api.Models
{

	[Table("ratings")]
	public class Rating : BaseModel
	{
		[PrimaryKey("id")]
		public int Id { get; set; }

		[Reference(typeof(Cocktail), useInnerJoin: false)]
		public ReferenceCocktailDto Cocktail { get; set; } = new ReferenceCocktailDto() { };

		[Column("cocktail_id")]
		public int CocktailId { get; set; }

		[Column("rating_value")]
		public int RatingValue { get; set; }

		[Column("user_id")]
		public string UserId { get; set; } = string.Empty;
		
		[Column("created_at")]
		public DateTime CreatedAt { get; set; }

		[Column("updated_at")]
		public DateTime UpdatedAt { get; set; }
	}
}
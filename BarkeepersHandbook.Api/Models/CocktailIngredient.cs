using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace BarkeepersHandbook.Api.Models
{
	[Table("cocktail_ingredients")]
	public class CocktailIngredient : BaseModel
	{
		[PrimaryKey("id", false)]
		public int Id { get; set; }

		[Column("cocktail_id")]
		public int CocktailId { get; set; }

		[Column("ingredient_id")]
		public int IngredientId { get; set; }

		[Reference(typeof(Ingredient))]
		public Ingredient Ingredient { get; set; } = new Ingredient();

		[Column("amount")]
		public double? Amount { get; set; }

		[Column("unit")]
		public string Unit { get; set; } = string.Empty;

		[Column("created_at")]
		public DateTime CreatedAt { get; set; }
	}
}
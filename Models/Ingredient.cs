using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace api.Models
{
	[Table("ingredients")]
	public class Ingredient : BaseModel
	{
		[PrimaryKey("id", false)]
		public int Id { get; set; }

		[Column("name")]
		public string Name { get; set; } = string.Empty;

		[Column("created_at")]
		public DateTime CreatedAt { get; set; }
	}
}
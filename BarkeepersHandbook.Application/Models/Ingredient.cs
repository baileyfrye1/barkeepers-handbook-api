using System;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace BarkeepersHandbook.Application.Models
{
	[Table("ingredients")]
	public class Ingredient : BaseModel
	{
		[PrimaryKey("id", false)]
		public int Id { get; init; }

		[Column("name")]
		public string Name { get; set; } = string.Empty;

		[Column("created_at")]
		public DateTime CreatedAt { get; set; }
	}
}
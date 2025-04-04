using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
		{

		}

		public DbSet<Cocktail> Cocktails { get; set; }
		public DbSet<CocktailIngredient> CocktailIngredients { get; set; }
		public DbSet<Ingredient> Ingredients { get; set; }

	}
}
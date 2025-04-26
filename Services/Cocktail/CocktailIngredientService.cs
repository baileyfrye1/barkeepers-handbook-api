using api.DTOs.CocktailIngredients;
using api.Mappers;
using api.Models;
using Supabase;

namespace api.Data
{
	public class CocktailIngredientService
	{
		private readonly Client _supabase;
		public CocktailIngredientService(Client supabase)
		{
			_supabase = supabase;
		}

		public async Task<CocktailIngredientDto> AddOneAsync(CocktailIngredient cocktailIngredient)
		{
			var result = await _supabase.From<CocktailIngredient>().Insert(cocktailIngredient);

			var newCocktailIngredient = result.Model.ToCocktailIngredientDto();

			return newCocktailIngredient;
		}

		public async Task<List<CocktailIngredientDto>> AddManyAsync(List<CocktailIngredient> cocktailIngredients)
		{
			var result = await _supabase.From<CocktailIngredient>().Insert(cocktailIngredients);

			var newCocktailIngredients = result.Models.Select(ci => ci.ToCocktailIngredientDto()).ToList();

			return newCocktailIngredients;
		}
	}
}
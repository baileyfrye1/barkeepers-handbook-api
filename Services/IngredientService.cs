using api.DTOs.Ingredients;
using api.Mappers;
using api.Models;
using Supabase;

namespace api.Data
{
	public class IngredientService
	{
		private readonly Client _supabase;

		public IngredientService(Client supabase)
		{
			_supabase = supabase;
		}

		public async Task<Ingredient> AddOneAsync(Ingredient ingredient)
		{
			var result = await _supabase.From<Ingredient>().Insert(ingredient);

			var newIngredient = result.Model;

			return newIngredient;
		}

		public async Task<Ingredient?> GetOneByNameAsync(string name)
		{
			var result = await _supabase.From<Ingredient>().Where(i => i.Name == name).Get();

			if (result.Model == null)
			{
				return null;
			}

			var ingredient = result.Model;

			return ingredient;
		}
	}
}

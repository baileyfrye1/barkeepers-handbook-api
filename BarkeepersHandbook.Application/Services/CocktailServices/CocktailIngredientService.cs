using BarkeepersHandbook.Application.Models;
using Newtonsoft.Json;
using Supabase;

namespace BarkeepersHandbook.Application.Services.CocktailServices;

public class CocktailIngredientService(Client supabase) : ICocktailIngredientService
{
	private readonly Client _supabase = supabase;

	public async Task<List<CocktailIngredient>> AddManyAsync(List<CocktailIngredient> cocktailIngredients)
	{
		var result = await _supabase.From<CocktailIngredient>().Insert(cocktailIngredients);

		var newCocktailIngredients = result.Models;

		return newCocktailIngredients;
	}
}

public interface ICocktailIngredientService
{
	Task<List<CocktailIngredient>> AddManyAsync(List<CocktailIngredient> cocktailIngredients);
}
using api.DTOs.CocktailIngredientDTOs;
using api.Mappers;
using api.Models;
using Supabase;

namespace api.Services.CocktailServices;

public class CocktailIngredientService : ICocktailIngredientService
{
	private readonly Client _supabase;
	public CocktailIngredientService(Client supabase)
	{
		_supabase = supabase;
	}

	public async Task<List<CocktailIngredientDto>> AddManyAsync(List<CocktailIngredient> cocktailIngredients)
	{
		var result = await _supabase.From<CocktailIngredient>().Insert(cocktailIngredients);

		var newCocktailIngredients = result.Models.Select(ci => ci.ToCocktailIngredientDto()).ToList();

		return newCocktailIngredients;
	}
}

public interface ICocktailIngredientService
{
	Task<List<CocktailIngredientDto>> AddManyAsync(List<CocktailIngredient> cocktailIngredients);
}
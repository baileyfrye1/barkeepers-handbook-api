using BarkeepersHandbook.Application.Errors;
using BarkeepersHandbook.Application.Models;
using BarkeepersHandbook.Application.Validators;
using FluentValidation;
using OneOf;
using OneOf.Types;
using Supabase;

namespace BarkeepersHandbook.Application.Services;

public class IngredientService : IIngredientService
{
	private readonly Client _supabase;
	private readonly IValidator<Ingredient> _validator;

	public IngredientService(Client supabase, IValidator<Ingredient> validator)
	{
		_supabase = supabase;
		_validator = validator;
	}

	public async Task<OneOf<Ingredient, ValidationFailed, UnexpectedError>> AddOneAsync(Ingredient ingredient)
	{
		var validationResult = await _validator.ValidateAsync(ingredient);

		if (!validationResult.IsValid)
		{
			return new ValidationFailed(validationResult.Errors);
		}
			
		var result = await _supabase.From<Ingredient>().Insert(ingredient);

		if (result.Model is null)
		{
			return new UnexpectedError("Error adding ingredient to database");
		}

		var newIngredient = result.Model;

		return newIngredient;
	}

	public async Task<OneOf<Ingredient, NotFound>> GetOneByNameAsync(string name)
	{
		var result = await _supabase.From<Ingredient>().Where(i => i.Name == name).Get();

		if (result.Model is null)
		{
			return new NotFound();
		}
			
		var ingredient = result.Model;

		return ingredient;
	}
}

public interface IIngredientService
{
	Task<OneOf<Ingredient, ValidationFailed, UnexpectedError>> AddOneAsync(Ingredient ingredient);
	Task<OneOf<Ingredient, NotFound>> GetOneByNameAsync(string name);
}
using BarkeepersHandbook.Api.Mappers;
using BarkeepersHandbook.Api.DTOs.IngredientDTOs;
using BarkeepersHandbook.Api.Errors;
using BarkeepersHandbook.Api.Models;
using BarkeepersHandbook.Api.Validators;
using FluentValidation;
using OneOf;
using OneOf.Types;
using Supabase;

namespace BarkeepersHandbook.Api.Services;

public class IngredientService : IIngredientService
{
	private readonly Client _supabase;
	private readonly IValidator<IngredientDto> _validator;

	public IngredientService(Client supabase, IValidator<IngredientDto> validator)
	{
		_supabase = supabase;
		_validator = validator;
	}

	public async Task<OneOf<Ingredient, ValidationFailed, UnexpectedError>> AddOneAsync(Ingredient ingredient)
	{
		var validationResult = await _validator.ValidateAsync(ingredient.ToIngredientDto());

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
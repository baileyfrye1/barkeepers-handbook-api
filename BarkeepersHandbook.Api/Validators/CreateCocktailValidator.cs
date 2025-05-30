using BarkeepersHandbook.Api.Models;
using BarkeepersHandbook.Api.DTOs.CocktailDTOs;
using FluentValidation;

namespace BarkeepersHandbook.Api.Validators
{
	public class CreateCocktailValidator : AbstractValidator<CreateCocktailRequestDto>
	{
		public CreateCocktailValidator()
		{
			RuleFor(c => c.Name).NotEmpty().WithMessage("Cocktail name is required");
			RuleFor(c => c.CocktailIngredients).NotEmpty().WithMessage("Cocktail ingredients are required");
		}
	}
}
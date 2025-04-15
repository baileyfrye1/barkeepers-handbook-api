using api.Models;
using FluentValidation;

namespace api.Validators
{
	public class CreateCocktailValidator : AbstractValidator<Cocktail>
	{
		public CreateCocktailValidator()
		{
			RuleFor(c => c.Name).NotEmpty().WithMessage("Cocktail name is required");
			RuleFor(c => c.CocktailIngredients).NotEmpty().WithMessage("Cocktail must have at least one ingredient");
		}
	}
}
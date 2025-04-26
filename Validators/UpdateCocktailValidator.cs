using api.DTOs.Cocktails;
using FluentValidation;

namespace api.Validators
{
	public class UpdateCocktailValidator : AbstractValidator<UpdateCocktailRequestDto>
	{
		public UpdateCocktailValidator()
		{
			RuleFor(c => c.Name).NotEmpty().When(c => !string.IsNullOrWhiteSpace(c.Name)).WithMessage("Cocktail name cannot be empty");
			RuleFor(c => c.Featured).NotEmpty().When(c => c.Featured.HasValue).WithMessage("Featured must be set to true or false");
			RuleFor(c => c.Tags).NotEmpty().When(c => c.Tags.Any()).WithMessage("Tags cannot be empty");
			RuleFor(c => c.CocktailIngredients).NotEmpty().When(c => c.CocktailIngredients.Any()).WithMessage("Cocktail ingredients cannot be empty");
		}
	}
}
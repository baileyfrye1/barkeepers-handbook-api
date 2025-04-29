using api.DTOs.IngredientDTOs;
using FluentValidation;

namespace api.Validators;

public class CreateIngredientValidator : AbstractValidator<IngredientDto>
{
    public CreateIngredientValidator()
    {
        RuleFor(i => i.Name).NotEmpty().WithMessage("Ingredient name is required");
    }
}
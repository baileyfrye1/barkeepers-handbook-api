using BarkeepersHandbook.Api.DTOs.IngredientDTOs;
using FluentValidation;

namespace BarkeepersHandbook.Api.Validators;

public class CreateIngredientValidator : AbstractValidator<IngredientDto>
{
    public CreateIngredientValidator()
    {
        RuleFor(i => i.Name).NotEmpty().WithMessage("Ingredient name is required");
    }
}
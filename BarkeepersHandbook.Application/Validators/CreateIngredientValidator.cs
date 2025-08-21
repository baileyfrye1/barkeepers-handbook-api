using BarkeepersHandbook.Application.Models;
using BarkeepersHandbook.Contracts.DTOs.IngredientDTOs;
using FluentValidation;

namespace BarkeepersHandbook.Application.Validators;

public class CreateIngredientValidator : AbstractValidator<Ingredient>
{
    public CreateIngredientValidator()
    {
        RuleFor(i => i.Name).NotEmpty().WithMessage("Ingredient name is required");
    }
}
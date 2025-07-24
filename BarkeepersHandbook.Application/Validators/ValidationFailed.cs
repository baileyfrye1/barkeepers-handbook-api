using FluentValidation.Results;

namespace BarkeepersHandbook.Application.Validators
{
    public record ValidationFailed(IEnumerable<ValidationFailure> Errors)
    {
        public ValidationFailed(ValidationFailure error) : this(new[] { error })
        { }
    }
}
using FluentValidation.Results;

namespace api.Validators
{
    public record ValidationFailed(IEnumerable<ValidationFailure> Errors)
    {
        public ValidationFailed(ValidationFailure error) : this(new[] { error })
        { }
    }
}
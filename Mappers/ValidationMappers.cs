using api.Validators;
using FluentValidation.Results;

namespace api.Mappers
{
    public static class ValidationMappers
    {
        public static ValidationFailureResponse MapToResponse(this ValidationFailed failed)
        {
            return new ValidationFailureResponse
            {
                Errors = failed.Errors.Select(x => new ValidationResponse
                {

                    PropertyName = x.PropertyName,
                    Message = x.ErrorMessage
                }).ToList()
            };
        }
    }
}
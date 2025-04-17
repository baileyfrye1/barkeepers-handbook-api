namespace api.Validators
{
	public class ValidationFailureResponse
	{
		public List<ValidationResponse> Errors { get; init; } = new();
	}
}

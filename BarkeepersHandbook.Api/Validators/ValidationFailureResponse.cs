namespace BarkeepersHandbook.Api.Validators
{
	public class ValidationFailureResponse
	{
		public List<ValidationResponse> Errors { get; init; } = new();
	}
}

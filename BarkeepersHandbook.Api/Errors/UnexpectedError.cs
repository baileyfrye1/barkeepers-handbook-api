namespace BarkeepersHandbook.Api.Errors
{
    public class UnexpectedError
    {
        public string Message { get; }

        public string? Details { get; }

        public UnexpectedError(string message, string? details = null)
        {
            Message = message;
            Details = details;
        }

        public override string ToString()
        {
            return Details is null ? Message : $"{Message} - {Details}";
        }
    }
}
namespace api.Errors;

public class AlreadyRated
{
    public string Message { get; }

    public AlreadyRated(string message = "You have already rated this cocktail")
    {
        Message = message;
    }
}
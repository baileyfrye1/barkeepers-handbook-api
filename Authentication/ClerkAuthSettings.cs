namespace api.Authentication;

public class ClerkAuthSettings
{
    public string SecretKey { get; set; }

    public ClerkAuthSettings(string secretKey)
    {
        SecretKey = secretKey;
    }
}
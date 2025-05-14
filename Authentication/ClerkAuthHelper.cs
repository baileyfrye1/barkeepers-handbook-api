using System.Text.Json;
using Clerk.BackendAPI.Helpers.Jwks;

namespace api.Authentication;

public class ClerkAuthHelper
{
    private readonly IConfiguration _config;

    public ClerkAuthHelper(IConfiguration config)
    {
        _config = config;
    }
    public static async Task<(bool IsSignedIn, RequestState State)> IsSignedInAsync(HttpRequest request, string secretKey)
    {
        if (request == null)
        {
            throw new InvalidOperationException("Request object is null.");
        }
        
        if (string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException("Clerk Secret Key is null or empty.");
        }
        
        var options = new AuthenticateRequestOptions(
            secretKey: secretKey,
            authorizedParties: ["http://localhost:3000", "https://barkeepershandbook.com"],
            jwtKey: request.Headers.Authorization.ToString()
        );
        
        var requestState = await AuthenticateRequest.AuthenticateRequestAsync(request, options);
        
        Console.WriteLine(requestState.Token);
        
        return (requestState.IsSignedIn(), requestState);
 
    }
}
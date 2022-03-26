using Microsoft.AspNetCore.Authentication;

namespace ChangeBlog.Api.Authentication;

public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public const string AuthenticationType = "API Key";
    public static string Scheme => "API Key";
}
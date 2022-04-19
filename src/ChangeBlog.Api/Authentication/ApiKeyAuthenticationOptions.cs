using Microsoft.AspNetCore.Authentication;

namespace ChangeBlog.Api.Authentication;

public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public const string AuthenticationType = "API Key";
    public const string Scheme = "API Key";
}
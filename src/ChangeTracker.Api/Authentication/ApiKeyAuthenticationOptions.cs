using Microsoft.AspNetCore.Authentication;

namespace ChangeTracker.Api.Authentication
{
    public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
    {
        public const string DefaultScheme = "API Key";
        public const string AuthenticationType = DefaultScheme;
        public static string Scheme => DefaultScheme;
    }
}
namespace ChangeBlog.Management.Api.Configuration;

// ReSharper disable once ClassNeverInstantiated.Global
public class AuthConfig
{
    public string Issuer { get; set; }
    public string TokenEndpoint { get; set; }
    public string RedirectUri { get; set; }
    public string LogoutUrl { get; set; }
    public bool Oidc { get; set; }
    public string ClientId { get; set; }
    public string ResponseType { get; set; }
    public string Scope { get; set; }
    public bool StrictDiscoveryDocumentValidation { get; set; }
    public bool ShowDebugInformation { get; set; }
}
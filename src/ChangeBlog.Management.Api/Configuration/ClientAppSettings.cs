namespace ChangeBlog.Management.Api.Configuration;

public class ClientAppSettings
{
    public string ChangeBlogApiBaseUrl { get; set; }
    public string DiscoveryDocument { get; set; }
    public AuthConfig AuthConfig { get; set; }
}
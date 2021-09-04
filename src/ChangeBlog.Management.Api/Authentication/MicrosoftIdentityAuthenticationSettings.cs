namespace ChangeBlog.Management.Api.Authentication
{
    public class MicrosoftIdentityAuthenticationSettings
    {
        public string Instance { get; set; }
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
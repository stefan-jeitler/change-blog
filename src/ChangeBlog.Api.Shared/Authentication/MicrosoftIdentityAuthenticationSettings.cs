﻿namespace ChangeBlog.Api.Shared.Authentication;

public class MicrosoftIdentityAuthenticationSettings
{
    public string Instance { get; set; }
    public string Domain { get; set; }
    public string SignUpSignInPolicyId { get; set; }
    public string TenantId { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
}
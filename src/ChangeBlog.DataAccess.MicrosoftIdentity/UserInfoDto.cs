using System.Text.Json.Serialization;

namespace ChangeBlog.DataAccess.MicrosoftIdentity;

public class UserInfoDto
{
    [JsonPropertyName("sub")] public string Subject { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("given_name")] public string GivenName { get; set; }

    [JsonPropertyName("family_name")] public string FamilyName { get; set; }

    [JsonPropertyName("email")] public string Email { get; set; }

    [JsonPropertyName("picture")] public string Picture { get; set; }
}
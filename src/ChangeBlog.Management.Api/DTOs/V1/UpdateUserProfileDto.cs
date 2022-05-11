namespace ChangeBlog.Management.Api.DTOs.V1;

public class UpdateUserProfileDto
{
    /// <summary>
    ///     Windows or Olson id
    /// </summary>
    public string Timezone { get; set; }

    public string Culture { get; set; }
}
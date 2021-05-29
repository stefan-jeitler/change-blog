using System.ComponentModel.DataAnnotations;

namespace ChangeTracker.Api.DTOs.V1.Version
{
    public class UpdateVersionDto
    {
        [Required] public string Name { get; set; }

        [Required] public string Version { get; set; }
    }
}
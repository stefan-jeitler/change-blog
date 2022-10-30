using System.ComponentModel.DataAnnotations;

namespace ChangeBlog.Management.Api.DTOs.V1.Account;

public class UpdateAccountDto
{
    [Required] public string Name { get; set; }
}
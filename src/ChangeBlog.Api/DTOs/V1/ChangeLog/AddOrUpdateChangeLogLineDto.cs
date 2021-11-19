using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ChangeBlog.Api.DTOs.V1.ChangeLog;

public class AddOrUpdateChangeLogLineDto
{
    [Required] public string Text { get; set; }

    public List<string> Labels { get; set; }

    public List<string> Issues { get; set; }
}
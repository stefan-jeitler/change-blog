using System;
using System.ComponentModel.DataAnnotations;

namespace ChangeBlog.Api.DTOs.V1.ChangeLog
{
    public class MoveChangeLogLineDto
    {
        [Required] public Guid TargetVersionId { get; set; }
    }
}

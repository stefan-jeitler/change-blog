using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeTracker.Api.DTOs.V1.ChangeLog
{
    public class MoveChangeLogLineDto
    {
        [Required]
        public Guid TargetVersionId { get; set; }
    }
}

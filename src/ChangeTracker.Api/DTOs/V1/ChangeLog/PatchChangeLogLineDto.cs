using System.Collections.Generic;

namespace ChangeTracker.Api.DTOs.V1.ChangeLog
{
    public class PatchChangeLogLineDto
    {
        public string Text { get; set; }
        public List<string> Labels { get; set; }
        public List<string> Issues { get; set; }
    }
}
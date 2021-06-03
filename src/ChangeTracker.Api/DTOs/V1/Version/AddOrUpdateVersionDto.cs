﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ChangeTracker.Api.DTOs.V1.ChangeLog;

namespace ChangeTracker.Api.DTOs.V1.Version
{
    public class AddOrUpdateVersionDto
    {
        public string Name { get; set; }

        [Required] public List<AddOrUpdateChangeLogLineDto> ChangeLogLines { get; set; }

        public bool ReleaseImmediately { get; set; }
    }
}
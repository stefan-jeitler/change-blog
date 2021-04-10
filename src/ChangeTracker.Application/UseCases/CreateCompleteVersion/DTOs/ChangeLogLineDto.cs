﻿using System.Collections.Generic;

namespace ChangeTracker.Application.UseCases.CreateCompleteVersion.DTOs
{
    public record ChangeLogLineDto(string Text, List<string> Labels, List<string> Issues);
}

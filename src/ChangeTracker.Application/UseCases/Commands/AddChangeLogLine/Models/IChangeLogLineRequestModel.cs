﻿using System;
using System.Collections.Generic;

namespace ChangeTracker.Application.UseCases.Commands.AddChangeLogLine.Models
{
    public interface IChangeLogLineRequestModel
    {
        Guid UserId { get; }
        string Text { get; }
        List<string> Labels { get; }
        List<string> Issues { get; }
    }
}
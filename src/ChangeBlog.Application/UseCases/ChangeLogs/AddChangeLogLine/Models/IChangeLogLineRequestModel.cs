using System;
using System.Collections.Generic;

namespace ChangeBlog.Application.UseCases.ChangeLogs.AddChangeLogLine.Models;

public interface IChangeLogLineRequestModel
{
    Guid UserId { get; }
    string Text { get; }
    List<string> Labels { get; }
    List<string> Issues { get; }
}
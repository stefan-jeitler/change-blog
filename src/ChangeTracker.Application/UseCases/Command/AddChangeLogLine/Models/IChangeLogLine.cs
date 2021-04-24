using System.Collections.Generic;

namespace ChangeTracker.Application.UseCases.Command.AddChangeLogLine.Models
{
    public interface IChangeLogLine
    {
        string Text { get; }
        List<string> Labels { get; }
        List<string> Issues { get; }
    }
}
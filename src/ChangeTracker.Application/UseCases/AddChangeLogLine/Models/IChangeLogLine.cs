using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.AddChangeLogLine.Models
{
    public interface IChangeLogLine
    {
        string Text { get; }
        List<string> Labels { get; }
        List<string> Issues { get; }
    }
}

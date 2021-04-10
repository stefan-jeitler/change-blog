using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChangeTracker.Application.Services.Issues;
using ChangeTracker.Application.Services.Labels;

namespace ChangeTracker.Application.UseCases.AddPendingChangeLogLine
{
    public interface IAddPendingChangeLogLineOutputPort : IExtractLabelsOutputPort, IExtractIssuesOutputPort
    {
        void InvalidChangeLogLineText(string text);
        void ProjectDoesNotExist();
        void MaxChangeLogLinesReached(int maxChangeLogLines);
        void Created(Guid changeLogLineId);
        void Conflict(string reason);
    }
}

using System;
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
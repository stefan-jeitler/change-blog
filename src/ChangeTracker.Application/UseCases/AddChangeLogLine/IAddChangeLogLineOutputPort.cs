using System;
using ChangeTracker.Application.Services.Issues;
using ChangeTracker.Application.Services.Labels;
using ChangeTracker.Application.Services.NotReleasedVersion;

namespace ChangeTracker.Application.UseCases.AddChangeLogLine
{
    public interface IAddChangeLogLineOutputPort
        : INotReleasedVersionOutputPort, IExtractLabelsOutputPort, IExtractIssuesOutputPort
    {
        void InvalidVersionFormat();
        void InvalidChangeLogLineText(string text);
        void MaxChangeLogLinesReached(int maxChangeLogLines);
        void Created(Guid changeLogLineId);
        void Conflict(string reason);
    }
}
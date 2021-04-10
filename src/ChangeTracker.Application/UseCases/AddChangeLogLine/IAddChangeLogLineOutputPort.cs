using System;
using System.Collections.Generic;
using ChangeTracker.Application.Services.Issues;
using ChangeTracker.Application.Services.Labels;
using ChangeTracker.Application.Services.NotReleasedVersion;

namespace ChangeTracker.Application.UseCases.AddChangeLogLine
{
    public interface IAddChangeLogLineOutputPort
        : INotReleasedVersionOutputPort, IExtractLabelsOutputPort, IExtractIssuesOutputPort
    {
        void InvalidVersionFormat();
        void InvalidChangeLogLine(string changeLogLine);
        void MaxChangeLogLinesReached(int maxChangeLog);
        void Created(Guid changeLogLineId);
        void Conflict(string reason);
    }
}
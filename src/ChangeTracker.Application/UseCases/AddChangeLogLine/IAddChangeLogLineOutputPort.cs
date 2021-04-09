using System;
using System.Collections.Generic;

namespace ChangeTracker.Application.UseCases.AddChangeLogLine
{
    public interface IAddChangeLogLineOutputPort
    {
        void ProjectDoesNotExist();
        void InvalidVersionFormat();
        void InvalidLabels(List<string> labels);
        void InvalidIssues(List<string> issues);
        void InvalidChangeLogLine(string changeLogLine);
        void TooManyLabels(int maxLabelsCount);
        void MaxChangeLogLinesReached(int maxChangeLog);
        void VersionDoesNotExist();
        void TooManyIssues(int maxIssues);
        void Created(Guid changeLogLineId);
        void Conflict(string reason);
        void VersionAlreadyReleased(DateTime releaseAt);
        void VersionDeleted(DateTime deletedAt);
    }
}
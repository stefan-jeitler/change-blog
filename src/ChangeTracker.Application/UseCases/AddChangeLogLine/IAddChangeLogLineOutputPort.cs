using System;
using System.Collections.Generic;

namespace ChangeTracker.Application.UseCases.AddChangeLogLine
{
    public interface IAddChangeLogLineOutputPort
    {
        void ProjectDoesNotExist();
        void InvalidVersionFormat();
        void InvalidLabels(List<string> labels);
        void InvalidChangeLogLine(string changeLogLine);
        void Created(Guid changeLogLineId);
        void Conflict(string reason);
        void TooManyLabels(int maxLabelsCount);
        void VersionHasTooManyChangeLog();
        void MaxChangeLogReached(int maxChangeLog);
        void VersionDoesNotExist();
    }
}
using System;
using ChangeTracker.Application.Services.Issues;
using ChangeTracker.Application.Services.Labels;

namespace ChangeTracker.Application.UseCases.CreateCompleteVersion
{
    public interface ICreateCompleteVersionOutputPort : IExtractLabelsOutputPort, IExtractIssuesOutputPort
    {
        void ProjectDoesNotExist();
        void InvalidVersionFormat(string version);
        void VersionDoesNotMatchScheme(string version);
        void MaxChangeLogLinesReached(int maxChangeLogLines);
        void InvalidChangeLogLineText(string text);
        void Created(Guid versionId);
        void Conflict(string reason);
    }
}
using System;
using System.Collections.Generic;
using ChangeTracker.Application.Services.ChangeLogLineParsing;

namespace ChangeTracker.Application.UseCases.Commands.AddOrUpdateVersion.OutputPorts
{
    public interface IAddVersionOutputPort : ILineParserOutput
    {
        void ProductDoesNotExist(Guid productId);
        void RelatedProductClosed(Guid productId);
        void InvalidVersionFormat(string version);
        void VersionDoesNotMatchScheme(string version, string versioningSchemeName);
        void Created(Guid versionId);
        void Conflict(string reason);
        void VersionAlreadyExists(Guid versionId);
        void TooManyLines(int maxChangeLogLines);
        void LinesWithSameTextsAreNotAllowed(IList<string> duplicates);
        void InvalidVersionName(string versionName);
    }
}
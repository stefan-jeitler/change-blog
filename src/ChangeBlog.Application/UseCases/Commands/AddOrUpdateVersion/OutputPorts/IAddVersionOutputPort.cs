using System;
using System.Collections.Generic;
using ChangeBlog.Application.DataAccess;
using ChangeBlog.Application.Services.ChangeLogLineParsing;

namespace ChangeBlog.Application.UseCases.Commands.AddOrUpdateVersion.OutputPorts
{
    public interface IAddVersionOutputPort : ILineParserOutput
    {
        void ProductDoesNotExist(Guid productId);
        void RelatedProductClosed(Guid productId);
        void InvalidVersionFormat(string version);
        void VersionDoesNotMatchScheme(string version, string versioningSchemeName);
        void Created(Guid versionId);
        void VersionAlreadyExists(Guid versionId);
        void TooManyLines(int maxChangeLogLines);
        void LinesWithSameTextsAreNotAllowed(IList<string> duplicates);
        void InvalidVersionName(string versionName);
        void InsertConflict(Conflict conflict);
    }
}

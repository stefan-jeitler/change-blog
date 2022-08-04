using System;
using System.Collections.Generic;
using ChangeBlog.Application.Boundaries.DataAccess;
using ChangeBlog.Application.ChangeLogLineParser;

namespace ChangeBlog.Application.UseCases.Versions.AddOrUpdateVersion.OutputPorts;

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
using System;
using System.Collections.Generic;
using ChangeBlog.Application.Boundaries.DataAccess;

namespace ChangeBlog.Application.UseCases.Commands.AssignAllPendingLinesToVersion;

public interface IAssignAllPendingLinesToVersionOutputPort
{
    void Assigned(Guid versionId);
    void InvalidVersionFormat(string version);
    void VersionDoesNotExist();
    void TooManyLinesToAdd(uint remainingLinesToAdd);
    void Conflict(Conflict conflict);
    void NoPendingChangeLogLines(Guid productId);
    void LineWithSameTextAlreadyExists(IEnumerable<string> texts);
    void TargetVersionBelongsToDifferentProduct(Guid productId, Guid targetVersionProductId);
}
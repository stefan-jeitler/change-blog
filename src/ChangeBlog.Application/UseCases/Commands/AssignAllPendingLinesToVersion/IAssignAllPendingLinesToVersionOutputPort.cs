using System;
using System.Collections.Generic;
using ChangeBlog.Application.DataAccess;

namespace ChangeBlog.Application.UseCases.Commands.AssignAllPendingLinesToVersion;

public interface IAssignAllPendingLinesToVersionOutputPort
{
    void Assigned(Guid versionId);
    void InvalidVersionFormat(string version);
    void VersionDoesNotExist();
    void TooManyLinesToAdd(uint remainingLinesToAdd);
    void Conflict(Conflict conflict);
    void NoPendingChangeLogLines(Guid productId);
    void LineWithSameTextAlreadyExists(List<string> texts);
    void TargetVersionBelongsToDifferentProduct(Guid productId, Guid targetVersionProductId);
}
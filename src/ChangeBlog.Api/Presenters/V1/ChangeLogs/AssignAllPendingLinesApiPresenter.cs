using System;
using System.Collections.Generic;
using ChangeBlog.Api.Localization.Resources;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Api.Shared.Presenters;
using ChangeBlog.Application.Boundaries.DataAccess;
using ChangeBlog.Application.UseCases.Commands.AssignAllPendingLinesToVersion;
using Microsoft.AspNetCore.Mvc;

namespace ChangeBlog.Api.Presenters.V1.ChangeLogs;

public class AssignAllPendingLinesApiPresenter : BaseApiPresenter, IAssignAllPendingLinesToVersionOutputPort
{
    public void Assigned(Guid versionId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.VersionId] = versionId.ToString()
        };

        Response = new OkObjectResult(SuccessResponse.Create(ChangeBlogStrings.ChangeLogLineMoved, resourceIds));
    }

    public void InvalidVersionFormat(string version)
    {
        Response = new UnprocessableEntityObjectResult(
            ErrorResponse.Create(string.Format(ChangeBlogStrings.InvalidVersionFormat, version)));
    }

    public void VersionDoesNotExist()
    {
        Response = new NotFoundObjectResult(ErrorResponse.Create(ChangeBlogStrings.VersionNotFound));
    }

    public void TooManyLinesToAdd(uint remainingLinesToAdd)
    {
        Response = new UnprocessableEntityObjectResult(
            ErrorResponse.Create(string.Format(ChangeBlogStrings.TooManyChangeLogLines_Remaining, remainingLinesToAdd)));
    }

    public void Conflict(Conflict conflict)
    {
        Response = conflict.ToResponse();
    }

    public void NoPendingChangeLogLines(Guid productId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.ProductId] = productId.ToString()
        };

        Response = new OkObjectResult(ErrorResponse.Create(ChangeBlogStrings.NoChangeLogLinesToAssign, resourceIds));
    }

    public void LineWithSameTextAlreadyExists(IEnumerable<string> texts)
    {
        Response = new UnprocessableEntityObjectResult(
            ErrorResponse.Create(string.Format(ChangeBlogStrings.TargetVersionContainsLinesWithSameText, string.Join(", ", texts))));
    }

    public void TargetVersionBelongsToDifferentProduct(Guid productId, Guid targetVersionProductId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.ProductId] = targetVersionProductId.ToString()
        };

        Response = new ConflictObjectResult(ErrorResponse.Create(ChangeBlogStrings.TargetVersionBelongsToDifferentProduct,
            resourceIds));
    }
}
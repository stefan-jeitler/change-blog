using System;
using System.Collections.Generic;
using System.Linq;
using ChangeBlog.Api.Localization.Resources;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Api.Shared.Presenters;
using ChangeBlog.Application.Boundaries.DataAccess;
using ChangeBlog.Application.UseCases.Commands.MakeAllChangeLogLinesPending;
using Microsoft.AspNetCore.Mvc;

namespace ChangeBlog.Api.Presenters.V1.ChangeLogs;

public class MakeAllChangeLogLinesPendingApiPresenter : BaseApiPresenter, IMakeAllChangeLogLinesPendingOutputPort
{
    public void VersionDoesNotExist()
    {
        Response = new NotFoundObjectResult(ErrorResponse.Create(ChangeBlogStrings.VersionNotFound));
    }

    public void VersionAlreadyReleased(Guid versionId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.VersionId] = versionId.ToString()
        };

        Response = new ConflictObjectResult(ErrorResponse.Create(ChangeBlogStrings.VersionAlreadyDeleted,
            resourceIds));
    }

    public void VersionDeleted(Guid versionId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.VersionId] = versionId.ToString()
        };

        Response = new ConflictObjectResult(ErrorResponse.Create(ChangeBlogStrings.VersionDeleted,
            resourceIds));
    }

    public void TooManyPendingLines(int maxChangeLogLines)
    {
        Response = new UnprocessableEntityObjectResult(
            ErrorResponse.Create(string.Format(ChangeBlogStrings.TooManyChangeLogLines, maxChangeLogLines)));
    }

    public void LineWithSameTextAlreadyExists(IEnumerable<string> duplicates)
    {
        var duplicatesFormatted = string.Join(", ", duplicates.Select(x => $"'{x}'"));
        var message = string.Format(ChangeBlogStrings.ChangeLogLineSameText, duplicatesFormatted);

        Response = new UnprocessableEntityObjectResult(
            ErrorResponse.Create(message));
    }

    public void Conflict(Conflict conflict)
    {
        Response = conflict.ToResponse();
    }

    public void MadePending(Guid productId, int count)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.ProductId] = productId.ToString()
        };

        Response = new OkObjectResult(SuccessResponse.Create(string.Format(ChangeBlogStrings.ChangeLogLinesMadePending, count), resourceIds));
    }

    public void InvalidVersionFormat(string version)
    {
        Response = new UnprocessableEntityObjectResult(ErrorResponse.Create(string.Format(ChangeBlogStrings.InvalidVersionFormat, version)));
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Application.DataAccess;
using ChangeBlog.Application.UseCases.Commands.MakeAllChangeLogLinesPending;
using Microsoft.AspNetCore.Mvc;

namespace ChangeBlog.Api.Presenters.V1.ChangeLogs;

public class MakeAllChangeLogLinesPendingApiPresenter : BaseApiPresenter, IMakeAllChangeLogLinesPendingOutputPort
{
    public void VersionDoesNotExist()
    {
        Response = new NotFoundObjectResult(DefaultResponse.Create("Version not found."));
    }

    public void VersionAlreadyReleased(Guid versionId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.VersionId] = versionId.ToString()
        };

        Response = new ConflictObjectResult(DefaultResponse.Create("The related version has already been released.",
            resourceIds));
    }

    public void VersionDeleted(Guid versionId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.VersionId] = versionId.ToString()
        };

        Response = new ConflictObjectResult(DefaultResponse.Create("The related version has been deleted.",
            resourceIds));
    }

    public void TooManyPendingLines(int maxChangeLogLines)
    {
        Response = new UnprocessableEntityObjectResult(
            DefaultResponse.Create($"Too many lines. Max lines: {maxChangeLogLines}"));
    }

    public void LineWithSameTextAlreadyExists(IEnumerable<string> duplicates)
    {
        var duplicatesFormatted = string.Join(", ", duplicates.Select(x => $"'{x}'"));

        Response = new UnprocessableEntityObjectResult(
            DefaultResponse.Create($"Lines with same text are not allowed. Duplicates: '{duplicatesFormatted}'"));
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

        Response = new OkObjectResult(DefaultResponse.Create($"{count} Lines were made pending.", resourceIds));
    }

    public void InvalidVersionFormat(string version)
    {
        Response = new UnprocessableEntityObjectResult(DefaultResponse.Create($"Invalid format '{version}'."));
    }
}
using System;
using System.Collections.Generic;
using ChangeBlog.Api.Shared;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Api.Shared.Presenters;
using ChangeBlog.Application.Boundaries.DataAccess;
using ChangeBlog.Application.UseCases.Commands.AddChangeLogLine;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChangeBlog.Api.Presenters.V1.ChangeLogs;

public class AddChangeLogLineApiPresenter : BaseApiPresenter, IAddChangeLogLineOutputPort
{
    private readonly HttpContext _httpContext;

    public AddChangeLogLineApiPresenter(IHttpContextAccessor httpContextAccessor)
    {
        _httpContext = httpContextAccessor.HttpContext;
    }

    public void InvalidVersionFormat(string version)
    {
        Response = new UnprocessableEntityObjectResult(ErrorResponse.Create($"Invalid format '{version}'."));
    }

    public void VersionDoesNotExist()
    {
        Response = new NotFoundObjectResult(ErrorResponse.Create("Version not found."));
    }

    public void LineWithSameTextAlreadyExists(Guid changeLogLineId, string duplicate)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.ChangeLogLineId] = changeLogLineId.ToString()
        };

        Response = new UnprocessableEntityObjectResult(
            ErrorResponse.Create($"Lines with same text are not allowed. Duplicate: '{duplicate}'", resourceIds));
    }

    public void Created(Guid changeLogLineId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.ChangeLogLineId] = changeLogLineId.ToString()
        };

        var location = _httpContext.CreateLinkTo($"api/v1/pending-changelogs/{changeLogLineId}");
        Response = new CreatedResult(location,
            SuccessResponse.Create("ChangeLogLine successfully added.", resourceIds));
    }

    public void Conflict(Conflict conflict)
    {
        Response = conflict.ToResponse();
    }

    public void TooManyLines(int maxChangeLogLines)
    {
        Response = new UnprocessableEntityObjectResult(
            ErrorResponse.Create($"Too many lines. Max lines: {maxChangeLogLines}"));
    }

    public void InvalidChangeLogLineText(string text)
    {
        Response = new BadRequestObjectResult(ErrorResponse.Create($"Invalid change log text '{text}'."));
    }

    public void InvalidIssue(string changeLogText, string issue)
    {
        Response = new BadRequestObjectResult(
            ErrorResponse.Create($"Invalid issue '{issue}' for change log '{changeLogText}'."));
    }

    public void TooManyIssues(string changeLogText, int maxIssues)
    {
        Response = new UnprocessableEntityObjectResult(
            ErrorResponse.Create(
                $"The change log '{changeLogText}' has too many issues. Max issues: '{maxIssues}'."));
    }

    public void InvalidLabel(string changeLogText, string label)
    {
        Response = new BadRequestObjectResult(
            ErrorResponse.Create($"Invalid label '{label}' for change log '{changeLogText}'."));
    }

    public void TooManyLabels(string changeLogText, int maxLabels)
    {
        Response = new UnprocessableEntityObjectResult(
            ErrorResponse.Create(
                $"The change log '{changeLogText}' has too many labels. Max labels: '{maxLabels}'."));
    }
}
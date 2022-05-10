using System;
using System.Collections.Generic;
using ChangeBlog.Api.Localization.Resources;
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

    public AddChangeLogLineApiPresenter(HttpContext httpContext)
    {
        _httpContext = httpContext;
    }

    public void InvalidVersionFormat(string version)
    {
        var message = string.Format(ChangeBlogStrings.InvalidFormat, version);
        Response = new UnprocessableEntityObjectResult(ErrorResponse.Create(message));
    }

    public void VersionDoesNotExist()
    {
        Response = new NotFoundObjectResult(ErrorResponse.Create(ChangeBlogStrings.VersionNotFound));
    }

    public void LineWithSameTextAlreadyExists(Guid changeLogLineId, string duplicate)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.ChangeLogLineId] = changeLogLineId.ToString()
        };

        var message = string.Format(ChangeBlogStrings.ChangeLogLineSameText, duplicate);
        Response = new UnprocessableEntityObjectResult(
            ErrorResponse.Create(message, resourceIds));
    }

    public void Created(Guid changeLogLineId)
    {
        var resourceIds = new Dictionary<string, string>
        {
            [KnownIdentifiers.ChangeLogLineId] = changeLogLineId.ToString()
        };

        var location = _httpContext.CreateLinkTo($"api/v1/pending-changelogs/{changeLogLineId}");
        Response = new CreatedResult(location,
            SuccessResponse.Create(ChangeBlogStrings.ChangeLogLineAdded, resourceIds));
    }

    public void Conflict(Conflict conflict)
    {
        Response = conflict.ToResponse();
    }

    public void TooManyLines(int maxChangeLogLines)
    {
        Response = new UnprocessableEntityObjectResult(
            ErrorResponse.Create(string.Format(ChangeBlogStrings.TooManyChangeLogLines, maxChangeLogLines)));
    }

    public void InvalidChangeLogLineText(string text)
    {
        Response = new BadRequestObjectResult(ErrorResponse.Create(string.Format(ChangeBlogStrings.InvalidChangeLogText, text)));
    }

    public void InvalidIssue(string changeLogText, string issue)
    {
        Response = new BadRequestObjectResult(
            ErrorResponse.Create(string.Format(ChangeBlogStrings.InvalidIssue, issue)));
    }

    public void TooManyIssues(string changeLogText, int maxIssues)
    {
        Response = new UnprocessableEntityObjectResult(
            ErrorResponse.Create(string.Format(ChangeBlogStrings.TooManyIssuesForChangeLogLine, changeLogText, maxIssues)));
    }

    public void InvalidLabel(string changeLogText, string label)
    {
        Response = new BadRequestObjectResult(
            ErrorResponse.Create(String.Format(ChangeBlogStrings.InvalidLabel, label)));
    }

    public void TooManyLabels(string changeLogText, int maxLabels)
    {
        Response = new UnprocessableEntityObjectResult(
            ErrorResponse.Create(string.Format(ChangeBlogStrings.TooManyLabelsForChangeLogLine, changeLogText, maxLabels)));
    }
}
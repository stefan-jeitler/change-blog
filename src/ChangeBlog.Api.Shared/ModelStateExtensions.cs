using System;
using System.Linq;
using ChangeBlog.Api.Shared.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ChangeBlog.Api.Shared;

public static class ModelStateExtensions
{
    public static ActionResult ToCustomErrorResponse(this ModelStateDictionary modelStateDictionary)
    {
        var errorMessages = modelStateDictionary
            .Where(x => x.Value is not null)
            .Select(modelStateEntry =>
                new PropertyErrorMessages(
                    // do not show trailing periods in error messages
                    modelStateEntry.Value.Errors.Select(x => x.ErrorMessage.Trim('.')).ToArray(),
                    modelStateEntry.Key.FirstCharToLower()))
            .Where(x => x.Messages.Any());

        return new BadRequestObjectResult(new ErrorResponse(errorMessages.ToArray()));
    }

    private static string FirstCharToLower(this string input) =>
        string.IsNullOrWhiteSpace(input)
            ? input
            : string.Concat(input[0].ToString().ToLower(), input.AsSpan(1));
}
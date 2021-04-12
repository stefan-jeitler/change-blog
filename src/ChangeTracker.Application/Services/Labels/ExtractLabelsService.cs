using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChangeTracker.Domain.ChangeLog;
using CSharpFunctionalExtensions;
// ReSharper disable InvertIf

namespace ChangeTracker.Application.Services.Labels
{
    public static class ExtractLabelsService
    {
        public static Maybe<List<Label>> Extract(IExtractLabelsOutputPort output, IEnumerable<string> labels,
            ChangeLogText text)
        {
            var parsedLabels = ParseLabels(labels);

            if (parsedLabels.IsFailure)
            {
                output.InvalidLabel(text, parsedLabels.Error);
                return Maybe<List<Label>>.None;
            }

            if (parsedLabels.Value.Count > ChangeLogLine.MaxLabels)
            {
                output.TooManyLabels(text, ChangeLogLine.MaxLabels);
                return Maybe<List<Label>>.None;
            }

            return Maybe<List<Label>>.From(parsedLabels.Value);
        }

        private static Result<List<Label>, string> ParseLabels(IEnumerable<string> labels)
        {
            var parsedLabels = new List<Label>();
            foreach (var l in labels)
            {
                if (Label.TryParse(l, out var label))
                {
                    parsedLabels.Add(label);
                }
                else
                {
                    return Result.Failure<List<Label>, string>(l);
                }
            }

            return Result.Success<List<Label>, string>(parsedLabels);
        }
    }
}

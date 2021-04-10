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
        public static Maybe<List<Label>> Extract(IExtractLabelsOutputPort output, IEnumerable<string> labels)
        {
            var (parsedLabels, invalidLabels) = ParseLabels(labels);

            if (invalidLabels.Any())
            {
                output.InvalidLabels(invalidLabels);
                return Maybe<List<Label>>.None;
            }

            if (parsedLabels.Count > ChangeLogLine.MaxLabels)
            {
                output.TooManyLabels(ChangeLogLine.MaxLabels);
                return Maybe<List<Label>>.None;
            }

            return Maybe<List<Label>>.From(parsedLabels);
        }

        private static (List<Label> parsedLabels, List<string> invalidLabels) ParseLabels(IEnumerable<string> labels)
        {
            var parsedLabels = new List<Label>();
            var invalidLabels = new List<string>();
            foreach (var l in labels)
            {
                if (Label.TryParse(l, out var label))
                {
                    parsedLabels.Add(label);
                }
                else
                {
                    invalidLabels.Add(l);
                }
            }

            return (parsedLabels, invalidLabels);
        }
    }
}

﻿namespace ChangeTracker.Application.Services.ChangeLog
{
    public interface IChangeLogLineParsingOutput
    {
        void InvalidChangeLogLineText(string text);
        void InvalidIssue(string changeLogText, string issue);
        void TooManyIssues(string changeLogText, int maxIssues);
        void InvalidLabel(string changeLogText, string labels);
        void TooManyLabels(string changeLogText, int maxLabelsCount);
        void TooManyLines(int maxChangeLogLines);
    }
}
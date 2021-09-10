namespace ChangeBlog.Application.ChangeLogLineParser
{
    public interface ILineParserOutput
    {
        void InvalidChangeLogLineText(string text);
        void InvalidIssue(string changeLogText, string issue);
        void TooManyIssues(string changeLogText, int maxIssues);
        void InvalidLabel(string changeLogText, string label);
        void TooManyLabels(string changeLogText, int maxLabels);
    }
}

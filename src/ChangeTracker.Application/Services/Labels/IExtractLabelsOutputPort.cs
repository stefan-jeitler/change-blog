namespace ChangeTracker.Application.Services.Labels
{
    public interface IExtractLabelsOutputPort
    {
        void InvalidLabel(string changeLogText, string labels);
        void TooManyLabels(string changeLogText, int maxLabelsCount);
    }
}
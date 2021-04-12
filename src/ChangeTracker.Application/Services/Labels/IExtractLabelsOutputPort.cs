using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChangeTracker.Domain.ChangeLog;

namespace ChangeTracker.Application.Services.Labels
{
    public interface IExtractLabelsOutputPort
    {
        void InvalidLabel(string changeLogText, string labels);
        void TooManyLabels(string changeLogText, int maxLabelsCount);
    }
}

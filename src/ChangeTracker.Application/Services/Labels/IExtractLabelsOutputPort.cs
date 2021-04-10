using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeTracker.Application.Services.Labels
{
    public interface IExtractLabelsOutputPort
    {
        void InvalidLabels(List<string> labels);
        void TooManyLabels(int maxLabelsCount);
    }
}

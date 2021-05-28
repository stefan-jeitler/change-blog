using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.Commands.UpdateVersion
{
    public interface IUpdateVersion
    {
        Task ExecuteAsync(IUpdateVersionOutputPort output, UpdateVersionRequestModel requestModel);
    }
}

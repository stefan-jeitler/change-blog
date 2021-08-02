using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.Queries.GetLatestVersion
{
    public interface IGetLatestVersion
    {
        Task ExecuteAsync(IGetLatestVersionOutputPort output, Guid userId, Guid productId);
    }
}

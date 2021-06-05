using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.Queries.GetLabels
{
    public interface IGetLabels
    {
        Task<IList<string>> ExecuteAsync(Guid changeLogLineId);
    }
}
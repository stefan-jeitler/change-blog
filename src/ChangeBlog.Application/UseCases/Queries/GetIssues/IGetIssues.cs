using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChangeBlog.Application.UseCases.Queries.GetIssues
{
    public interface IGetIssues
    {
        Task<IList<string>> ExecuteAsync(Guid changeLogLineId);
    }
}

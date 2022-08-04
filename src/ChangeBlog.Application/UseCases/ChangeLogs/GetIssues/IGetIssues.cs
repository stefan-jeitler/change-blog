using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChangeBlog.Application.UseCases.ChangeLogs.GetIssues;

public interface IGetIssues
{
    Task<IList<string>> ExecuteAsync(Guid changeLogLineId);
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChangeBlog.Application.UseCases.ChangeLogs.GetLabels;

public interface IGetLabels
{
    Task<IList<string>> ExecuteAsync(Guid changeLogLineId);
}
using System;
using System.Threading.Tasks;

namespace ChangeBlog.Application.UseCases.ChangeLogs.ChangeLogLineExists;

public interface IChangeLogLineExists
{
    Task<bool> ExecuteAsync(Guid changeLogLineId);
}
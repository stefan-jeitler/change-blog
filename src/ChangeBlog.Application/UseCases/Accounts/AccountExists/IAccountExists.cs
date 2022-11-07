using System;
using System.Threading.Tasks;

namespace ChangeBlog.Application.UseCases.Accounts.AccountExists;

public interface IAccountExists
{
    Task<bool> ExecuteAsync(Guid accountId);
}
using System;
using System.Threading.Tasks;

namespace ChangeBlog.Application.UseCases.Accounts.DeleteAccount;

public interface IDeleteAccount
{
    Task ExecuteAsync(IDeleteAccountOutputPort output, Guid accountId);
}
using System;
using ChangeBlog.Application.Boundaries.DataAccess;

namespace ChangeBlog.Application.UseCases.Accounts.DeleteAccount;

public interface IDeleteAccountOutputPort
{
    void AccountDeleted(Guid accountId);
    void Conflict(Conflict conflict);
}
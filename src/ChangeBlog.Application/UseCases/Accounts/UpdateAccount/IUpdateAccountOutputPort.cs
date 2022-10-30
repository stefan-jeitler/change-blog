using System;
using ChangeBlog.Application.Boundaries.DataAccess;

namespace ChangeBlog.Application.UseCases.Accounts.UpdateAccount;

public interface IUpdateAccountOutputPort
{
    void NewNameAlreadyTaken(string newAccountName);
    void Conflict(Conflict conflict);
    void Updated(Guid accountId);
}
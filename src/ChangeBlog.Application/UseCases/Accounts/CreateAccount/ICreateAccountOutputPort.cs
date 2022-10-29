using System;
using ChangeBlog.Application.Boundaries.DataAccess;

namespace ChangeBlog.Application.UseCases.Accounts.CreateAccount;

public interface ICreateAccountOutputPort
{
    void Created(Guid accountId);
    void InvalidName(string name);
    void Conflict(Conflict conflict);
    void AccountAlreadyExists(Guid accountId);
    void TooManyAccountsCreated(ushort limit);
}
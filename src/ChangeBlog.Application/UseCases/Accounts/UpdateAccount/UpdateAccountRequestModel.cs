using System;

namespace ChangeBlog.Application.UseCases.Accounts.UpdateAccount;

public class UpdateAccountRequestModel
{
    public UpdateAccountRequestModel(Guid accountId, string name)
    {
        if (accountId == Guid.Empty)
            throw new ArgumentException("AccountId must not be empty");

        AccountId = accountId;
        Name = name;
    }

    public Guid AccountId { get; }
    public string Name { get; }
}
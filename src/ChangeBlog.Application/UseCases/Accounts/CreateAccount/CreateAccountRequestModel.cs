using System;

namespace ChangeBlog.Application.UseCases.Accounts.CreateAccount;

public class CreateAccountRequestModel
{
    public CreateAccountRequestModel(string name, Guid userId)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));

        if (userId == Guid.Empty)
            throw new ArgumentException("UserId must not be empty");

        UserId = userId;
    }

    public string Name { get; }
    public Guid UserId { get; }
}
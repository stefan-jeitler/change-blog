using System;
using System.Collections.Generic;

namespace ChangeBlog.Domain;

public class AccountUser
{
    public AccountUser(Account account, User user, IReadOnlyList<Role> roles, DateTime createdAt)
    {
        Account = account ?? throw new ArgumentNullException(nameof(account));
        User = user ?? throw new ArgumentNullException(nameof(user));
        Roles = roles ?? throw new ArgumentNullException(nameof(roles));

        if (createdAt == DateTime.MinValue || createdAt == DateTime.MaxValue)
            throw new ArgumentException("Invalid creation date");

        CreatedAt = createdAt;
    }

    public Account Account { get; }
    public User User { get; }
    public IReadOnlyList<Role> Roles { get; }

    public DateTime CreatedAt { get; set; }
}
using System;
using ChangeBlog.Application.UseCases.Accounts.GetAccounts;

namespace ChangeBlog.Management.Api.DTOs.V1.Account;

public class AccountDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string DefaultVersioningScheme { get; set; }
    public Guid DefaultVersioningSchemeId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public static AccountDto FromResponseModel(AccountResponseModel m)
    {
        return new AccountDto
        {
            Id = m.Id,
            Name = m.Name,
            DefaultVersioningScheme = m.DefaultVersioningScheme,
            DefaultVersioningSchemeId = m.DefaultVersioningSchemeId,
            CreatedAt = m.CreatedAt
        };
    }
}
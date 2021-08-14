using System;
using ChangeBlog.Application.UseCases.Queries.GetAccounts;

namespace ChangeBlog.Api.DTOs.V1.Account
{
    public class AccountDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DefaultVersioningScheme { get; set; }
        public Guid DefaultVersioningSchemeId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        public static AccountDto FromResponseModel(AccountResponseModel m) =>
            new()
            {
                Id = m.Id,
                Name = m.Name,
                DefaultVersioningScheme = m.DefaultVersioningScheme,
                DefaultVersioningSchemeId = m.DefaultVersioningSchemeId,
                CreatedAt = m.CreatedAt
            };
    }
}

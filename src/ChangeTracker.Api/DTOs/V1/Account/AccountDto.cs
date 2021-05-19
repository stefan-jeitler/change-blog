using System;
using ChangeTracker.Application.UseCases.Queries.GetAccounts;

namespace ChangeTracker.Api.DTOs.V1.Account
{
    public class AccountDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DefaultVersioningScheme { get; set; }
        public Guid DefaultVersioningSchemeId { get; set; }
        public DateTime CreatedAt { get; set; }

        public static AccountDto FromResponseModel(AccountResponseModel m)
        {
            return new()
            {
                Id = m.Id,
                Name = m.Name,
                DefaultVersioningScheme = m.DefaultVersioningScheme,
                DefaultVersioningSchemeId = m.DefaultVersioningSchemeId,
                CreatedAt = m.CreatedAt
            };
        }
    }
}
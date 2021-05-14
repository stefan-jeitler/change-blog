using System;

namespace ChangeTracker.Application.UseCases.Queries.GetAccounts
{
    public class AccountResponseModel
    {
        public AccountResponseModel(Guid id, string name, string defaultVersioningScheme,
            Guid defaultVersioningSchemeId, DateTime createdAt)
        {
            Id = id;
            Name = name;
            DefaultVersioningScheme = defaultVersioningScheme;
            DefaultVersioningSchemeId = defaultVersioningSchemeId;
            CreatedAt = createdAt;
        }

        public Guid Id { get; }
        public string Name { get; }
        public string DefaultVersioningScheme { get; }
        public Guid DefaultVersioningSchemeId { get; }
        public DateTime CreatedAt { get; }
    }
}
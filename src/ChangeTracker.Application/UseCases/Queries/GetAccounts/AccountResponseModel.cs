using System;

namespace ChangeTracker.Application.UseCases.Queries.GetAccounts
{
    public class AccountResponseModel
    {
        public AccountResponseModel(Guid id, string name, string defaultVersioningScheme,
            Guid defaultVersioningSchemeId, DateTime createdAt)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id cannot be empty.");

            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            DefaultVersioningScheme = defaultVersioningScheme ?? throw new ArgumentNullException(nameof(defaultVersioningScheme));

            if (DefaultVersioningSchemeId == Guid.Empty)
                throw new ArgumentException("VersioningSchemeId cannot be empty.");

            DefaultVersioningSchemeId = defaultVersioningSchemeId;

            if (createdAt == DateTime.MinValue || createdAt == DateTime.MaxValue)
                throw new ArgumentException("Invalid creation date.");

            CreatedAt = createdAt;
        }

        public Guid Id { get; }
        public string Name { get; }
        public string DefaultVersioningScheme { get; }
        public Guid DefaultVersioningSchemeId { get; }
        public DateTime CreatedAt { get; }
    }
}
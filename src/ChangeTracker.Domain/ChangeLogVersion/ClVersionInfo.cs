using System;

// ReSharper disable ConvertIfStatementToSwitchStatement

namespace ChangeTracker.Domain.ChangeLogVersion
{
    public class ClVersionInfo : IEquatable<ClVersionInfo>
    {
        public ClVersionInfo(Guid projectId, ClVersion version, DateTime? releasedAt)
            : this(Guid.NewGuid(), projectId, version, releasedAt, DateTime.UtcNow, null)
        {
        }

        public ClVersionInfo(Guid id, Guid projectId, ClVersion version, DateTime? releasedAt, DateTime createdAt,
            DateTime? deletedAt)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id must not be empty.");

            Id = id;

            if (projectId == Guid.Empty)
                throw new ArgumentException("ProjectId must not be empty.");

            ProjectId = projectId;

            Value = version ?? throw new ArgumentNullException(nameof(version));

            if (releasedAt.HasValue &&
                (releasedAt.Value == DateTime.MinValue || releasedAt.Value == DateTime.MaxValue))
            {
                throw new ArgumentException("Invalid release date", nameof(releasedAt));
            }

            ReleasedAt = releasedAt;

            if (createdAt == DateTime.MinValue || createdAt == DateTime.MaxValue)
                throw new ArgumentException("Invalid creation date.");

            CreatedAt = createdAt;

            if (deletedAt.HasValue &&
                (deletedAt.Value == DateTime.MinValue || deletedAt.Value == DateTime.MaxValue))
                throw new ArgumentException("Invalid deletion date.");

            DeletedAt = deletedAt;
        }

        public Guid Id { get; }
        public Guid ProjectId { get; }
        public ClVersion Value { get; }
        public DateTime? ReleasedAt { get; }
        public DateTime CreatedAt { get; }
        public DateTime? DeletedAt { get; }

        public bool WasAlreadyReleased => ReleasedAt.HasValue;

        public bool Equals(ClVersionInfo other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Id.Equals(other.Id) &&
                   ProjectId.Equals(other.ProjectId) &&
                   Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            return obj.GetType() == GetType() &&
                   Equals((ClVersionInfo) obj);
        }

        public override int GetHashCode() => HashCode.Combine(Id, ProjectId, Value);
    }
}
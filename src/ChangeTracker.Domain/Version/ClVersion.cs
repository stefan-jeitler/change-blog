using System;

namespace ChangeTracker.Domain.Version
{
    public class ClVersion : IEquatable<ClVersion>
    {
        public ClVersion(Guid projectId, ClVersionValue versionValue, DateTime? releasedAt = null, DateTime? deletedAt = null)
            : this(Guid.NewGuid(), projectId, versionValue, releasedAt, DateTime.UtcNow, deletedAt)
        {
        }

        public ClVersion(Guid id, Guid projectId, ClVersionValue versionValue, DateTime? releasedAt, DateTime createdAt,
            DateTime? deletedAt)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id must not be empty.");

            Id = id;

            if (projectId == Guid.Empty)
                throw new ArgumentException("ProjectId must not be empty.");

            ProjectId = projectId;
            Value = versionValue ?? throw new ArgumentNullException(nameof(versionValue));

            if (releasedAt.HasValue &&
                (releasedAt.Value == DateTime.MinValue || releasedAt.Value == DateTime.MaxValue))
            {
                throw new ArgumentException("Invalid release date.", nameof(releasedAt));
            }

            ReleasedAt = releasedAt;

            if (createdAt == DateTime.MinValue || createdAt == DateTime.MaxValue)
                throw new ArgumentException("Invalid creation date.");

            CreatedAt = createdAt;

            VerifyDeletedAtDate(releasedAt, deletedAt);
            DeletedAt = deletedAt;
        }

        public Guid Id { get; }
        public Guid ProjectId { get; }
        public ClVersionValue Value { get; }
        public DateTime? ReleasedAt { get; }
        public DateTime CreatedAt { get; }
        public DateTime? DeletedAt { get; }

        public bool IsReleased => ReleasedAt.HasValue;

        public bool IsDeleted => DeletedAt.HasValue;

        public bool Equals(ClVersion other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Id.Equals(other.Id) &&
                   ProjectId.Equals(other.ProjectId) &&
                   Equals(Value, other.Value);
        }

        private static void VerifyDeletedAtDate(DateTime? releasedAt, DateTime? deletedAt)
        {
            if (deletedAt.HasValue &&
                (deletedAt.Value == DateTime.MinValue || deletedAt.Value == DateTime.MaxValue))
                throw new ArgumentException("Invalid deletion date.");

            if (deletedAt.HasValue && releasedAt.HasValue &&
                deletedAt.Value < releasedAt.Value)
                throw new InvalidOperationException("You cannot release a deleted version.");
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            return obj.GetType() == GetType() &&
                   Equals((ClVersion) obj);
        }

        public override int GetHashCode() => HashCode.Combine(Id, ProjectId, Value);
    }
}
namespace SIGTI.Domain.Common
{
    public abstract class BaseEntity : Entity
    {
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; private set; }

        protected void UpdateTimestamp()
        {
            UpdatedAt = DateTime.UtcNow;

        }
    }
}

using SIGTI.Domain.Common;
using SIGTI.Domain.Exceptions;

namespace SIGTI.Domain.Entities
{
    public sealed class SupportQueueMember : BaseEntity
    {
        private const int MinMaxConcurrentTickets = 1;
        public Guid SupportQueueId { get; private set; }
        public SupportQueue SupportQueue { get; private set; } = null!;
        public Guid TechnicianId { get; private set; }
        public User Technician { get; private set; } = null!;
        public bool IsActive { get; private set; } = true;
        public DateTime JoinedAt { get; private set; }
        public DateTime? LeftAt { get; private set; }
        public int MaxConcurrentTickets { get; private set; }

        private SupportQueueMember() { }

        public SupportQueueMember(
            SupportQueue supportQueue,
            User technician,
            int maxConcurrentTickets
        )
        {
            SupportQueue = supportQueue;
            SupportQueueId = supportQueue.Id;
            Technician = technician;
            TechnicianId = technician.Id;

            JoinedAt = DateTime.UtcNow;

            ValidateMaxConcurrentTickets(maxConcurrentTickets);
            MaxConcurrentTickets = maxConcurrentTickets;
            IsActive = true;
        }

        public void UpdateMaxConcurrentTickets(int maxConcurrentTickets)
        {
            if (maxConcurrentTickets < MinMaxConcurrentTickets)
                throw new DomainException("A capacidade máxima deve ser maior que zero.");

            MaxConcurrentTickets = maxConcurrentTickets;

            UpdateTimestamp();
        }

        private static void ValidateMaxConcurrentTickets(int maxConcurrentTickets)
        {
            if (maxConcurrentTickets <= 0)
                throw new DomainException("O limite de chamados deve ser maior que zero.");
        }

        public void Activate(int maxConcurrentTickets)
        {
            if (IsActive)
                throw new DomainException("O membro já está ativo");

            ValidateMaxConcurrentTickets(maxConcurrentTickets);

            MaxConcurrentTickets = maxConcurrentTickets;
            IsActive = true;
            LeftAt = null;
            JoinedAt = DateTime.UtcNow;
            UpdateTimestamp();
        }

        public void Deactivate()
        {
            if (!IsActive)
                throw new DomainException("O membro já está inativo.");

            IsActive = false;
            LeftAt = DateTime.UtcNow;
            UpdateTimestamp();
        }
    }
}

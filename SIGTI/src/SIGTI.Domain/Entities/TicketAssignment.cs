using SIGTI.Domain.Common;
using SIGTI.Domain.Enums;
using SIGTI.Domain.Exceptions;

namespace SIGTI.Domain.Entities
{
    public class TicketAssignment : BaseEntity
    {
        public Guid TicketId { get; private set; }
        public Ticket Ticket { get; private set; } = null!;
        public Guid TechnicianId { get; private set; }
        public User Technician { get; private set; } = null!;
        public Guid AssignedById { get; private set; }
        public User AssignedBy { get; private set; } = null!;
        public DateTime AssignedAt { get; private set; }
        public DateTime? FinishedAt { get; private set; }
        public string Reason { get; private set; }
        public bool IsFinished => FinishedAt.HasValue;

        private TicketAssignment() { } // For EF Core

        public TicketAssignment(Ticket ticket, User technician, User assignedBy, string reason)
        {
            if (ticket is null)
                throw new DomainException("O ticket é obrigatório.");

            if (technician is null)
                throw new DomainException("O técnico é obrigatório.");

            if (!technician.IsTechnician() && !technician.IsAdministrator())
                throw new DomainException("O usuário informado não é um técnico/administrador");

            if (!technician.IsActive)
                throw new DomainException("O técnico está desativado.");

            if (assignedBy is null)
                throw new DomainException("O usuário pela atribuição é obrigatório.");

            if (!assignedBy.IsActive)
                throw new DomainException("O usuário pela atribuição está desativado.");

            Ticket = ticket;
            TicketId = ticket.Id;

            Technician = technician;
            TechnicianId = technician.Id;

            AssignedBy = assignedBy;
            AssignedById = assignedBy.Id;

            UpdateReason(reason);

            AssignedAt = DateTime.UtcNow;
        }

        public void UpdateReason(string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
                throw new DomainException("A razão da atribuição não pode ser vazia.");
            Reason = reason.Trim();
            UpdateTimestamp();
        }

        public void MarkAsFinished()
        {
            if (FinishedAt.HasValue)
                throw new DomainException("A atribuição já foi finalizada.");
            FinishedAt = DateTime.UtcNow;
            UpdateTimestamp();
        }
    }
}

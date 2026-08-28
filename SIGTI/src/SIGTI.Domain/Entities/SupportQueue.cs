using SIGTI.Domain.Common;
using SIGTI.Domain.Enums;
using SIGTI.Domain.Exceptions;

namespace SIGTI.Domain.Entities
{
    public sealed class SupportQueue : BaseEntity
    {
        private const int MaxNameLength = 150;
        private const int MaxDescriptionLength = 500;
        public string Name { get; private set; }
        public string Description { get; private set; }
        public bool IsActive { get; private set; } = true;
        private readonly List<SupportQueueMember> _members = [];
        public IReadOnlyCollection<SupportQueueMember> Members => _members;
        private readonly List<Ticket> _tickets = [];
        public IReadOnlyCollection<Ticket> Tickets => _tickets;

        private SupportQueue() { } // For EF Core

        public SupportQueue(string name, string description)
        {
            UpdateName(name);
            UpdateDescription(description);
        }

        public void UpdateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("O nome da fila de suporte é obrigatório.");

            name = name.Trim();

            if (name.Length > MaxNameLength)
                throw new DomainException(
                    $"O nome da fila de suporte deve ter no máximo {MaxNameLength} caracteres."
                );
            Name = name;

            UpdateTimestamp();
        }

        public void UpdateDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new DomainException("A descrição da fila de suporte é obrigatória.");

            description = description.Trim();
            if (description.Length > MaxDescriptionLength)
                throw new DomainException(
                    $"A descrição da fila de suporte deve ter no máximo {MaxDescriptionLength} caracteres."
                );

            Description = description;
            UpdateTimestamp();
        }

        public void Activate()
        {
            if (IsActive)
                throw new DomainException("A fila já está ativa.");

            IsActive = true;
            UpdateTimestamp();
        }

        public void Deactivate()
        {
            if (!IsActive)
                throw new DomainException("A fila já está inativa.");

            IsActive = false;
            UpdateTimestamp();
        }

        public void AddMember(User technician, int maxConcurrentTickets)
        {
            if (technician is null)
                throw new DomainException("O técnico é obrigatório.");

            if (technician.IsTechnician() is false)
                throw new DomainException(
                    "O usuário deve ser um técnico para ser adicionado à fila."
                );

            var member = _members.FirstOrDefault(x => x.TechnicianId == technician.Id);

            if (member is not null)
            {
                if (member.IsActive)
                    throw new DomainException("O técnico já é membro ativo da fila.");

                member.Activate(maxConcurrentTickets);
                UpdateTimestamp();
                return;
            }

            _members.Add(new SupportQueueMember(this, technician, maxConcurrentTickets));

            UpdateTimestamp();
        }

        public void RemoveMember(User technician)
        {
            if (technician is null)
                throw new DomainException("O técnico é obrigatório.");

            if (technician.IsTechnician() is false)
                throw new DomainException(
                    "O usuário deve ser um técnico para ser removido da fila."
                );

            var member = _members.FirstOrDefault(m =>
                m.TechnicianId == technician.Id && m.IsActive
            );

            if (member is null)
                throw new DomainException("O técnico não é membro da fila.");

            member.Deactivate();

            UpdateTimestamp();
        }

        public int GetActiveMemberCount() => _members.Count(m => m.IsActive);
    }
}

using SIGTI.Domain.Common;
using SIGTI.Domain.Exceptions;

namespace SIGTI.Domain.Entities
{
    public sealed class Department : BaseEntity
    {
        private const int MaxNameLength = 150;
        private const int MaxDescriptionLength = 500;
        public string Name { get; private set; }
        public string Description { get; private set; }
        public bool IsActive { get; private set; } = true;
        private readonly List<User> _users = [];
        public IReadOnlyCollection<User> Users => _users;
        private readonly List<Ticket> _tickets = [];
        public IReadOnlyCollection<Ticket> Tickets => _tickets;

        private Department() { } // For EF Core

        public Department(string name, string description)
        {
            UpdateName(name);
            UpdateDescription(description);
        }

        public void Update(string name, string description)
        {
            UpdateName(name);
            UpdateDescription(description);
        }

        public void UpdateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("O nome do departamento é obrigatório.");

            Name = name.Trim();

            if (name.Length > MaxNameLength)
                throw new DomainException(
                    $"O nome do departamento deve possuir no máximo {MaxNameLength} caracteres."
                );

            UpdateTimestamp();
        }

        public void UpdateDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new DomainException("A descrição do departamento é obrigatória.");

            if (description.Length > MaxDescriptionLength)
                throw new DomainException(
                    "Descrição ultrapassa a quantidade de caracteres permitidos."
                );

            Description = description.Trim();
            UpdateTimestamp();
        }

        public void Activate()
        {
            if (IsActive)
                throw new DomainException("O departamento já está ativo.");

            IsActive = true;
            UpdateTimestamp();
        }

        public void Deactivate()
        {
            if (!IsActive)
                throw new DomainException("O departamento já está inativo.");

            IsActive = false;
            UpdateTimestamp();
        }
    }
}

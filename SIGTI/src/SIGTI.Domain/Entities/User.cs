using SIGTI.Domain.Common;
using SIGTI.Domain.Enums;
using SIGTI.Domain.Exceptions;
using SIGTI.Domain.ValueObjects;

namespace SIGTI.Domain.Entities
{
    public sealed class User : BaseEntity
    {
        private const int MinNameLength = 10;
        private const int MaxNameLength = 100;

        public string Name { get; private set; }
        public Email Email { get; private set; }
        public string PasswordHash { get; private set; }
        public Role Role { get; private set; }
        public Department Department { get; private set; } = null!;
        public Guid DepartmentId { get; private set; }
        public bool IsActive { get; private set; } = true;
        public bool IsSystem { get; private set; }

        // Navigation Properties
        private readonly List<Ticket> _createdTickets = [];
        public IReadOnlyCollection<Ticket> CreatedTickets => _createdTickets;
        private readonly List<TicketAssignment> _assignedTickets = [];
        public IReadOnlyCollection<TicketAssignment> AssignedTickets => _assignedTickets;
        private readonly List<Comment> _comments = [];
        public IReadOnlyCollection<Comment> Comments => _comments;
        private readonly List<SupportQueueMember> _queueMemberships = [];
        public IReadOnlyCollection<SupportQueueMember> QueueMemberships => _queueMemberships;
        private readonly List<TicketAssignment> _assignmentsMade = [];
        public IReadOnlyCollection<TicketAssignment> AssignmentsMade => _assignmentsMade;

        private User() { } // For EF Core

        public User(string name, Email email, string passwordHash, Role role, Department department)
        {
            UpdateName(name);
            UpdateEmail(email);
            UpdatePasswordHash(passwordHash);
            UpdateRole(role);
            ChangeDepartment(department);
        }

        private void UpdateEmail(Email email)
        {
            Email = email ?? throw new DomainException(nameof(email));
            UpdateTimestamp();
        }

        public void UpdateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("O nome é obrigatório.");

            Name = name.Trim();

            if (name.Length < MinNameLength || name.Length > MaxNameLength)
                throw new DomainException(
                    $"O nome deve ter entre {MinNameLength} e {MaxNameLength} caracteres."
                );

            UpdateTimestamp();
        }

        public void UpdatePasswordHash(string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new DomainException("A senha é obrigatória.");
            PasswordHash = passwordHash.Trim();
            UpdateTimestamp();
        }

        public void UpdateRole(Role role)
        {
            if (!Enum.IsDefined(typeof(Role), role))
                throw new DomainException("Função inválida.");

            if (IsSystem)
                throw new DomainException("Não é possível alterar a função de um usuário System.");

            Role = role;
            UpdateTimestamp();
        }

        public void ChangeDepartment(Department department)
        {
            if (department is null)
                throw new DomainException("O departamento é obrigatório.");
            Department = department;
            DepartmentId = department.Id;
            UpdateTimestamp();
        }

        public void Deactivate()
        {
            if (!IsActive)
                throw new DomainException("O usuário já está desativado.");
            IsActive = false;
            UpdateTimestamp();
        }

        public void Activate()
        {
            if (IsActive)
                throw new DomainException("O usuário já está ativado.");
            IsActive = true;
            UpdateTimestamp();
        }

        public bool IsTechnician() => Role == Role.Technician;

        public bool IsAdministrator() => Role == Role.Administrator;

        public bool IsUser() => Role == Role.User;

        internal void MarkAsSystem()
        {
            IsSystem = true;
        }
    }
}

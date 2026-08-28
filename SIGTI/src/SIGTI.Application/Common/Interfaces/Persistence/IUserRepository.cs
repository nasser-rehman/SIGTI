using System.ComponentModel;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Enums;
using SIGTI.Domain.ValueObjects;

namespace SIGTI.Application.Common.Interfaces.Persistence
{
    public interface IUserRepository
    {
        Task AddAsync(User user, CancellationToken cancellationToken);
        Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken);
        Task<User?> GetSystemUserAsync(CancellationToken cancellationToken);
        Task<bool> ExistsAsync(Guid userId, CancellationToken cancellationToken);
        Task<bool> ExistsByEmailASync(Email email, CancellationToken cancellationToken);
        Task<IReadOnlyCollection<User>> ListAllAsync(CancellationToken cancellationToken);
        Task<IReadOnlyCollection<User>> ListByDepartmentAsync(
            Guid departmentId,
            CancellationToken cancellationToken
        );
        Task<IReadOnlyCollection<User>> ListByRoleAsync(
            Role role,
            CancellationToken cancellationToken
        );
        Task<IReadOnlyCollection<User>> ListTechnicians(CancellationToken cancellationToken);
        Task<IReadOnlyCollection<User>> ListAdministrators(CancellationToken cancellationToken);
        Task<IReadOnlyCollection<User>> ListUsers(CancellationToken cancellationToken);
        Task<IReadOnlyCollection<User>> ListActiveTechniciansAsync(
            CancellationToken cancellationToken
        );
    }
}

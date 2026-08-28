using SIGTI.Domain.Entities;

namespace SIGTI.Application.Common.Interfaces.Persistence
{
    public interface IDepartmentRepository
    {
        Task<Department?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<IReadOnlyCollection<Department>> ListAllAsync(CancellationToken cancellationToken);
        Task AddAsync(Department department, CancellationToken cancellationToken);
        Task<bool> ExistsAsync(Guid departmentId, CancellationToken cancellationToken);
        Task<bool> ExistsByNameAsync(string departmentName, CancellationToken cancellationToken);
        Task<IReadOnlyCollection<Department>> ListActiveAsync(CancellationToken cancellationToken);
    }
}

using Microsoft.EntityFrameworkCore;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Domain.Entities;
using SIGTI.Infrastructure.Persistence.Context;

namespace SIGTI.Infrastructure.Persistence.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly ApplicationDbContext _context;

        public DepartmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Department department, CancellationToken cancellationToken)
        {
            await _context.Departments.AddAsync(department, cancellationToken);
        }

        public Task<Department?> GetByIdAsync(
            Guid departmentId,
            CancellationToken cancellationToken
        )
        {
            return _context.Departments.FirstOrDefaultAsync(
                department => department.Id == departmentId,
                cancellationToken
            );
        }

        public async Task<IReadOnlyCollection<Department>> ListAllAsync(
            CancellationToken cancellationToken
        )
        {
            return await _context
                .Departments.AsNoTracking()
                .OrderBy(department => department.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsAsync(Guid departmentId, CancellationToken cancellationToken)
        {
            return await _context
                .Departments.AsNoTracking()
                .AnyAsync(department => department.Id == departmentId, cancellationToken);
        }

        public async Task<bool> ExistsByNameAsync(
            string departmentName,
            CancellationToken cancellationToken
        )
        {
            return await _context
                .Departments.AsNoTracking()
                .AnyAsync(department => department.Name == departmentName, cancellationToken);
        }

        public async Task<IReadOnlyCollection<Department>> ListActiveAsync(
            CancellationToken cancellationToken
        )
        {
            return await _context
                .Departments.AsNoTracking()
                .Where(department => department.IsActive)
                .OrderBy(department => department.Name)
                .ToListAsync(cancellationToken);
        }
    }
}

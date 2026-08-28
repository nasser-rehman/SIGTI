using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Enums;
using SIGTI.Domain.ValueObjects;
using SIGTI.Infrastructure.Persistence.Context;

namespace SIGTI.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(User user, CancellationToken cancellationToken)
        {
            await _context.Users.AddAsync(user, cancellationToken);
        }

        public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return _context.Users.FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
        }

        public Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken)
        {
            return _context
                .Users.AsNoTracking()
                .FirstOrDefaultAsync(user => user.Email == email, cancellationToken);
        }

        public async Task<User?> GetSystemUserAsync(CancellationToken cancellationToken)
        {
            return await _context.Users.FirstOrDefaultAsync(
                user => user.IsSystem,
                cancellationToken
            );
        }

        public async Task<bool> ExistsAsync(Guid userId, CancellationToken cancellationToken)
        {
            var foundUser = await _context
                .Users.AsNoTracking()
                .FirstOrDefaultAsync(user => user.Id == userId, cancellationToken);

            if (foundUser is not null)
                return true;

            return false;
        }

        public async Task<bool> ExistsByEmailASync(Email email, CancellationToken cancellationToken)
        {
            var foundUser = await _context
                .Users.AsNoTracking()
                .FirstOrDefaultAsync(user => user.Email == email);

            if (foundUser is not null)
                return true;

            return false;
        }

        public async Task<IReadOnlyCollection<User>> ListAllAsync(
            CancellationToken cancellationToken
        )
        {
            return await _context.Users.AsNoTracking().OrderBy(user => user.Name).ToListAsync();
        }

        public async Task<IReadOnlyCollection<User>> ListByDepartmentAsync(
            Guid departmentId,
            CancellationToken cancellationToken
        )
        {
            return await _context
                .Users.AsNoTracking()
                .Where(user => user.DepartmentId == departmentId)
                .OrderBy(user => user.Name)
                .ToListAsync();
        }

        public async Task<IReadOnlyCollection<User>> ListByRoleAsync(
            Role role,
            CancellationToken cancellationToken
        )
        {
            return await _context
                .Users.AsNoTracking()
                .Where(user => user.Role == role)
                .OrderBy(user => user.Name)
                .ToListAsync();
        }

        public async Task<IReadOnlyCollection<User>> ListTechnicians(
            CancellationToken cancellationToken
        )
        {
            return await _context
                .Users.AsNoTracking()
                .Where(user => user.Role == Role.Technician)
                .ToListAsync();
        }

        public async Task<IReadOnlyCollection<User>> ListAdministrators(
            CancellationToken cancellationToken
        )
        {
            return await _context
                .Users.AsNoTracking()
                .Where(user => user.Role == Role.Administrator)
                .ToListAsync();
        }

        public async Task<IReadOnlyCollection<User>> ListUsers(CancellationToken cancellationToken)
        {
            return await _context
                .Users.AsNoTracking()
                .Where(user => user.Role == Role.User)
                .ToListAsync();
        }

        public async Task<IReadOnlyCollection<User>> ListActiveTechniciansAsync(
            CancellationToken cancellationToken
        )
        {
            return await _context
                .Users.AsNoTracking()
                .Where(user => user.IsActive == true)
                .OrderBy(user => user.Name)
                .ToListAsync();
        }
    }
}

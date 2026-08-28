using Microsoft.EntityFrameworkCore;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Domain.Entities;
using SIGTI.Infrastructure.Persistence.Context;

namespace SIGTI.Infrastructure.Persistence.Repositories
{
    public class SupportQueueRepository : ISupportQueueRepository
    {
        private readonly ApplicationDbContext _context;

        public SupportQueueRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(SupportQueue supportQueue, CancellationToken cancellationToken)
        {
            await _context.SupportQueues.AddAsync(supportQueue, cancellationToken);
        }

        public async Task<bool> ExistsAsync(Guid queueId, CancellationToken cancellationToken)
        {
            return await _context
                .SupportQueues.AsNoTracking()
                .AnyAsync(queue => queue.Id == queueId, cancellationToken);
        }

        public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken)
        {
            return await _context
                .SupportQueues.AsNoTracking()
                .AnyAsync(queue => queue.Name == name, cancellationToken);
        }

        public async Task<IReadOnlyCollection<SupportQueue>> GetAllAsync(
            CancellationToken cancellationToken
        )
        {
            return await _context
                .SupportQueues.AsNoTracking()
                .OrderBy(supportQueue => supportQueue.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<SupportQueue?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context
                .SupportQueues.Include(queue => queue.Members)
                    .ThenInclude(member => member.Technician)
                .FirstOrDefaultAsync(queue => queue.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyCollection<SupportQueue>> ListActiveAsync(
            CancellationToken cancellationToken
        )
        {
            return await _context
                .SupportQueues.AsNoTracking()
                .Where(queue => queue.IsActive)
                .OrderBy(queue => queue.Name)
                .ToListAsync(cancellationToken);
        }
    }
}

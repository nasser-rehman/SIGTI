using SIGTI.Domain.Entities;

namespace SIGTI.Application.Common.Interfaces.Persistence
{
    public interface ISupportQueueRepository
    {
        Task<SupportQueue?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        Task<IReadOnlyCollection<SupportQueue>> GetAllAsync(CancellationToken cancellationToken);

        Task AddAsync(SupportQueue supportQueue, CancellationToken cancellationToken);
        Task<bool> ExistsAsync(Guid queueId, CancellationToken cancellationToken);
        Task<IReadOnlyCollection<SupportQueue>> ListActiveAsync(
            CancellationToken cancellationToken
        );
        Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken);
    }
}

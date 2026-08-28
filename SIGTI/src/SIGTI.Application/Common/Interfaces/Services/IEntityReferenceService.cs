using SIGTI.Domain.Entities;

namespace SIGTI.Application.Common.Interfaces.Services
{
    public interface IEntityReferenceService
    {
        Task<User> GetRequiredUserAsync(
        Guid id,
        CancellationToken cancellationToken);

        Task<Department> GetRequiredDepartmentAsync(
            Guid id,
            CancellationToken cancellationToken);

        Task<SupportQueue> GetRequiredQueueAsync(
            Guid id,
            CancellationToken cancellationToken);

        Task<Ticket> GetRequiredTicketAsync(
            Guid id,
            CancellationToken cancellationToken);

        Task<User> GetRequiredSystemUserAsync(CancellationToken cancellationToken);
    }

}

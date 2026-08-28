using SIGTI.Domain.Entities;

namespace SIGTI.Application.Common.Interfaces.Services
{
    public interface ITechnicianAssignmentService
    {
        Task<SupportQueueMember> SelectTechnicianAsync(
            SupportQueue queue,
            CancellationToken cancellationToken = default
        );
    }
}

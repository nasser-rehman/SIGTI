using MediatR;

namespace SIGTI.Application.Features.SupportQueues.Commands.UpdateMemberCapacity
{
    public sealed record UpdateMemberCapacityCommand(
        Guid QueueId,
        Guid TechnicianId,
        int MaxConcurrentTickets
    ) : IRequest<UpdateMemberCapacityResponse>;
}

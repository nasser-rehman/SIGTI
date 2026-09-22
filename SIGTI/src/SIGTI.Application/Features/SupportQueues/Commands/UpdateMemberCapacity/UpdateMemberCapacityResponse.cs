namespace SIGTI.Application.Features.SupportQueues.Commands.UpdateMemberCapacity
{
    public sealed record UpdateMemberCapacityResponse(
        Guid QueueId,
        Guid TechnicianId,
        int MaxConcurrentTickets,
        DateTime? UpdatedAt
    );
}

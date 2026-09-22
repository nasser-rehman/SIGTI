namespace SIGTI.Application.Features.SupportQueues.Commands.RemoveMember
{
    public sealed record RemoveMemberResponse(
        Guid QueueId,
        Guid TechnicianId,
        bool IsActive
    );
}

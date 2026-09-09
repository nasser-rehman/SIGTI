namespace SIGTI.Application.Features.SupportQueues.Commands.AddMember
{
    public sealed record AddMemberRequest(
        Guid TechnicianId,
        int MaxConcurrentTickets
    );
}

using MediatR;

namespace SIGTI.Application.Features.SupportQueues.Commands.RemoveMember
{
    public sealed record RemoveMemberCommand(Guid QueueId, Guid TechnicianId)
        : IRequest<RemoveMemberResponse>;
}

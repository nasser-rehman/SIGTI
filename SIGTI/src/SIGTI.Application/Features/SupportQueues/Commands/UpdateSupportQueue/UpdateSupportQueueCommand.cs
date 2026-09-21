using MediatR;

namespace SIGTI.Application.Features.SupportQueues.Commands.UpdateSupportQueue
{
    public sealed record UpdateSupportQueueCommand(
        Guid Id,
        string Name,
        string Description
    ) : IRequest<UpdateSupportQueueResponse>;
}

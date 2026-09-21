using MediatR;

namespace SIGTI.Application.Features.SupportQueues.Commands.ActivateSupportQueue
{
    public sealed record ActivateSupportQueueCommand(Guid Id)
        : IRequest<ActivateSupportQueueResponse>;
}

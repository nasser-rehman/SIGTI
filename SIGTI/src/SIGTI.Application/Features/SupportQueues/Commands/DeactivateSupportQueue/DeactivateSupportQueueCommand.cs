using MediatR;

namespace SIGTI.Application.Features.SupportQueues.Commands.DeactivateSupportQueue
{
    public sealed record DeactivateSupportQueueCommand(Guid Id)
        : IRequest<DeactivateSupportQueueResponse>;
}

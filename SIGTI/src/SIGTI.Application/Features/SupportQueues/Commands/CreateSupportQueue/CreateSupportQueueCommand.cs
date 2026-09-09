using MediatR;

namespace SIGTI.Application.Features.SupportQueues.Commands.CreateSupportQueue
{
    public sealed record CreateSupportQueueCommand(
        string Name,
        string Description
    ) : IRequest<CreateSupportQueueResponse>;
}

namespace SIGTI.Application.Features.SupportQueues.Commands.CreateSupportQueue
{
    public sealed record CreateSupportQueueResponse(
        Guid Id,
        string Name,
        string Description
    );
}

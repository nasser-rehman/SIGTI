namespace SIGTI.Application.Features.SupportQueues.Commands.CreateSupportQueue
{
    public sealed record CreateSupportQueueRequest(
        string Name,
        string Description
    );
}

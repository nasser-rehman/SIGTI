namespace SIGTI.Application.Features.SupportQueues.Commands.UpdateSupportQueue
{
    public sealed record UpdateSupportQueueRequest(
        string Name,
        string Description
    );
}

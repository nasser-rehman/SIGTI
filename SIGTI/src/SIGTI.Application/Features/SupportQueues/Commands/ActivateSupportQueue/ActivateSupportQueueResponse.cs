namespace SIGTI.Application.Features.SupportQueues.Commands.ActivateSupportQueue
{
    public sealed record ActivateSupportQueueResponse(
        Guid Id,
        bool IsActive,
        DateTime? UpdatedAt
    );
}

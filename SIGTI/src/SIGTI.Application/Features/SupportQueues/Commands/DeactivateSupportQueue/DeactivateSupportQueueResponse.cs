namespace SIGTI.Application.Features.SupportQueues.Commands.DeactivateSupportQueue
{
    public sealed record DeactivateSupportQueueResponse(
        Guid Id,
        bool IsActive,
        DateTime? UpdatedAt
    );
}

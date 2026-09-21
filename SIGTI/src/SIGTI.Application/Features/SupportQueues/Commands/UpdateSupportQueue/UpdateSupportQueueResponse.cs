namespace SIGTI.Application.Features.SupportQueues.Commands.UpdateSupportQueue
{
    public sealed record UpdateSupportQueueResponse(
        Guid Id,
        string Name,
        string Description,
        bool IsActive,
        DateTime? UpdatedAt
    );
}

namespace SIGTI.Application.Features.SupportQueues.Queries.GetSupportQueueById
{
    public sealed record GetSupportQueueByIdResponse(
        Guid Id,
        string Name,
        string Description,
        bool IsActive,
        DateTime CreatedAt,
        DateTime? UpdatedAt,
        IReadOnlyCollection<SupportQueueMemberResponse> Members
    );

    public sealed record SupportQueueMemberResponse(
        Guid TechnicianId,
        string TechnicianName,
        string Email,
        int MaxConcurrentTickets,
        bool IsActive,
        DateTime JoinedAt,
        DateTime? LeftAt
    );
}

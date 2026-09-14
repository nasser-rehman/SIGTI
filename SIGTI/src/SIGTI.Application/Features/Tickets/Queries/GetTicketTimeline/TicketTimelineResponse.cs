using SIGTI.Domain.Enums;

namespace SIGTI.Application.Features.Tickets.Queries.GetTicketTimeline
{
    public sealed record TicketTimelineResponse(
        Guid TicketId,
        string TicketCode,
        string Title,
        TicketStatus Status,
        IReadOnlyList<TicketTimelineItemResponse> Timeline
    );
}

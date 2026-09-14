namespace SIGTI.Application.Features.Tickets.Queries.GetTicketTimeline
{
    public sealed record TicketTimelineItemResponse(
        string EventType,
        string Description,
        DateTime Timestamp,
        Guid? ActorId,
        string? ActorName
    );
}

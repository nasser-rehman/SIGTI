using MediatR;

namespace SIGTI.Application.Features.Tickets.Queries.GetTicketTimeline
{
    public sealed record GetTicketTimelineQuery(Guid TicketId)
        : IRequest<TicketTimelineResponse>;
}

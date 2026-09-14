using MediatR;
using SIGTI.Application.Common.Interfaces.Services;

namespace SIGTI.Application.Features.Tickets.Queries.GetTicketTimeline
{
    public sealed class GetTicketTimelineQueryHandler
        : IRequestHandler<GetTicketTimelineQuery, TicketTimelineResponse>
    {
        private readonly IEntityReferenceService _entityReferenceService;

        public GetTicketTimelineQueryHandler(
            IEntityReferenceService entityReferenceService
        )
        {
            _entityReferenceService = entityReferenceService;
        }

        public async Task<TicketTimelineResponse> Handle(
            GetTicketTimelineQuery request,
            CancellationToken cancellationToken
        )
        {
            var ticket = await _entityReferenceService.GetRequiredTicketAsync(
                request.TicketId,
                cancellationToken
            );

            var events = new List<TicketTimelineItemResponse>();

            // 1. Created Event
            events.Add(
                new TicketTimelineItemResponse(
                    EventType: "Created",
                    Description: $"Chamado criado no departamento '{ticket.DepartmentName}'.",
                    Timestamp: ticket.CreatedAt,
                    ActorId: ticket.CreatedById,
                    ActorName: ticket.CreatedBy?.Name
                )
            );

            // 2. Assignment, dispatch, transfer events

            foreach (var assignment in ticket.Assignments)
            {
                events.Add(
                    new TicketTimelineItemResponse(
                        EventType: "Assigned",
                        Description: $"Atribuido ao técnico '{assignment.Technician.Name}'. Motivo: {assignment.Reason}",
                        Timestamp: assignment.AssignedAt,
                        ActorId: assignment.AssignedById,
                        ActorName: assignment.AssignedBy?.Name
                    )
                );
            }

            // 3. Start service event
            if (ticket.FirstResponseAt.HasValue)
            {
                events.Add(
                    new TicketTimelineItemResponse(
                        EventType: "Started",
                        Description: "Atendimento técnico iniciado.",
                        Timestamp: ticket.FirstResponseAt.Value,
                        ActorId: ticket.CurrentAssignment?.TechnicianId,
                        ActorName: ticket.CurrentAssignment?.Technician?.Name
                    )
                );
            }

            // 4. Comments events
            foreach (var comment in ticket.Comments)
            {
                events.Add(
                    new TicketTimelineItemResponse(
                        EventType: "CommentAdded",
                        Description: comment.Content,
                        Timestamp: comment.CreatedAt,
                        ActorId: comment.AuthorId,
                        ActorName: comment.Author?.Name
                    )
                );
            }

            // 5. Resolved Event
            if (ticket.ResolvedAt.HasValue)
            {
                events.Add(
                    new TicketTimelineItemResponse(
                        EventType: "Resolved",
                        Description: "Chamado marcado como resolvido.",
                        Timestamp: ticket.ResolvedAt.Value,
                        ActorId: ticket.CurrentAssignment?.TechnicianId,
                        ActorName: ticket.CurrentAssignment?.Technician?.Name
                    )
                );
            }

            // 6. Closed Event
            if (ticket.ClosedAt.HasValue)
            {
                events.Add(
                    new TicketTimelineItemResponse(
                        EventType: "Closed",
                        Description: "Chamado encerrado.",
                        Timestamp: ticket.ClosedAt.Value,
                        ActorId: null,
                        ActorName: null
                    )
                );
            }

            // Strictly chronological order
            var orderedTimeline = events.OrderBy(e => e.Timestamp).ToList();

            return new TicketTimelineResponse(
                ticket.Id,
                ticket.Code,
                ticket.Title,
                ticket.Status,
                orderedTimeline
            );
        }
    }
}

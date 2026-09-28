using SIGTI.Domain.Enums;

namespace SIGTI.Application.Features.Tickets.Commands.ReclassifyTicket
{
    public sealed record ReclassifyTicketResponse(
        Guid Id,
        TicketPriority Priority,
        TicketCategory Category,
        DateTime? UpdatedAt
    );
}

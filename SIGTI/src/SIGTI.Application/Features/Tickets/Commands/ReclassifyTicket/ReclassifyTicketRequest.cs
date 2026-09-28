using SIGTI.Domain.Enums;

namespace SIGTI.Application.Features.Tickets.Commands.ReclassifyTicket
{
    public sealed record ReclassifyTicketRequest(
        TicketPriority Priority,
        TicketCategory Category
    );
}

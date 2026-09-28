using MediatR;
using SIGTI.Domain.Enums;

namespace SIGTI.Application.Features.Tickets.Commands.ReclassifyTicket
{
    public sealed record ReclassifyTicketCommand(
        Guid TicketId,
        TicketPriority Priority,
        TicketCategory Category
    ) : IRequest<ReclassifyTicketResponse>;
}

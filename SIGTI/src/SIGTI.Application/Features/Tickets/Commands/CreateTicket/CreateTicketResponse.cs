using SIGTI.Domain.Enums;

namespace SIGTI.Application.Features.Tickets.Commands.CreateTicket
{
    public sealed record CreateTicketResponse(
        Guid Id,
        string Code,
        string Title,
        TicketStatus Status
    );

}

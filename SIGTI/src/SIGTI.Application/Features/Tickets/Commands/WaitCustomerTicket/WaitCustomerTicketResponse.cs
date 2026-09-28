using SIGTI.Domain.Enums;

namespace SIGTI.Application.Features.Tickets.Commands.WaitCustomerTicket
{
    public sealed record WaitCustomerTicketResponse(
        Guid Id,
        TicketStatus Status,
        DateTime? UpdatedAt
    );
}

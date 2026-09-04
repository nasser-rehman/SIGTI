using SIGTI.Domain.Enums;

namespace SIGTI.Application.Features.Tickets.Commands.StartTicketService
{
    public sealed record StartTicketServiceResponse(
        Guid Id,
        TicketStatus Status,
        DateTime? UpdatedAt
    );
}

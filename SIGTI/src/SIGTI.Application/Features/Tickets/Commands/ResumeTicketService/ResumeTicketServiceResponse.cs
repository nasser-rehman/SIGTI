using SIGTI.Domain.Enums;

namespace SIGTI.Application.Features.Tickets.Commands.ResumeTicketService
{
    public sealed record ResumeTicketServiceResponse(
        Guid Id,
        TicketStatus Status,
        DateTime? UpdatedAt
    );
}

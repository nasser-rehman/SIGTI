using SIGTI.Domain.Enums;

namespace SIGTI.Application.Features.Tickets.Commands.DispatchTicket
{
    public sealed record DispatchTicketResponse(
        Guid TicketId,
        Guid TechnicianId,
        string TechnicianName,
        TicketStatus Status,
        DateTime AssignedAt
    );
}

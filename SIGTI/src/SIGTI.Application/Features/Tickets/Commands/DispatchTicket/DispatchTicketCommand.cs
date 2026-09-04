using MediatR;

namespace SIGTI.Application.Features.Tickets.Commands.DispatchTicket
{
    public sealed record DispatchTicketCommand(
        Guid TicketId,
        Guid? TechnicianId,
        Guid AssignedById,
        string? Reason = null
    ) : IRequest<DispatchTicketResponse>;
}

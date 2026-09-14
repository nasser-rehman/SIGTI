namespace SIGTI.Application.Features.Tickets.Commands.DispatchTicket
{
    public sealed record DispatchTicketRequest(
        Guid? TechnicianId,
        string? Reason
    );
}

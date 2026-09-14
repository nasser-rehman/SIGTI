using SIGTI.Domain.Enums;

namespace SIGTI.Application.Features.Tickets.Commands.CreateTicket
{
    public sealed record CreateTicketRequest(
        string Title,
        string Description,
        TicketPriority Priority,
        TicketCategory Category,
        Guid DepartmentId,
        Guid QueueId
    );
}

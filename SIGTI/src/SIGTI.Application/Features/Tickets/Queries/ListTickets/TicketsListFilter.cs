using SIGTI.Domain.Enums;

namespace SIGTI.Application.Features.Tickets.Queries.ListTickets
{
    public sealed record TicketListFilter
    {
        public TicketStatus? Status { get; init; }
        public TicketPriority? Priority { get; init; }
        public TicketCategory? Category { get; init; }
        public Guid? DepartmentId { get; init; }
        public Guid? QueueId { get; init; }
        public Guid? TechnicianId { get; init; }
    }
}

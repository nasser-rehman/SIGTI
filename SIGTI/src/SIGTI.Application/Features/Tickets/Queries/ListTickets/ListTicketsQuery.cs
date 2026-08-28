using MediatR;
using SIGTI.Application.Common.Enums;
using SIGTI.Application.Common.Models;
using SIGTI.Application.Common.Requests;

namespace SIGTI.Application.Features.Tickets.Queries.ListTickets
{
    public sealed record ListTicketsQuery() : PagedQuery, IRequest<PagedResult<ListTicketsResponse>>
    {
        public TicketListFilter Filter { get; init; } = new();
        public TicketSortField SortBy { get; init; } = TicketSortField.CreatedAt;
        public SortDirection SortDirection { get; init; } = SortDirection.Descending;
    }
}

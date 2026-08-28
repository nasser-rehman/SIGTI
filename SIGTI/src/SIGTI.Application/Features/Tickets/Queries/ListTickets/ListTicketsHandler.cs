using MediatR;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Application.Common.Models;

namespace SIGTI.Application.Features.Tickets.Queries.ListTickets
{
    public sealed class ListTicketsHandler
        : IRequestHandler<ListTicketsQuery, PagedResult<ListTicketsResponse>>
    {
        private readonly ITicketRepository _ticketRepository;

        public ListTicketsHandler(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        public async Task<PagedResult<ListTicketsResponse>> Handle(
            ListTicketsQuery request,
            CancellationToken cancellationToken
        )
        {
            var skip = (request.Page - 1) * request.PageSize;
            var tickets = await _ticketRepository.ListAsync(
                request.Filter,
                request.SortBy,
                request.SortDirection,
                skip,
                request.PageSize,
                cancellationToken
            );
            var totalCount = await _ticketRepository.CountAsync(request.Filter, cancellationToken);

            var ticketResponses = tickets
                .Select(ticket => new ListTicketsResponse(
                    ticket.Id,
                    ticket.Number,
                    ticket.Title,
                    ticket.Status.ToString(),
                    ticket.Priority.ToString(),
                    ticket.Category.ToString(),
                    ticket.Department.Name,
                    ticket.Queue.Name,
                    ticket.CurrentAssignment?.Technician.Name,
                    ticket.CreatedAt
                ))
                .ToList();

            return new PagedResult<ListTicketsResponse>(
                ticketResponses,
                request.Page,
                request.PageSize,
                totalCount
            );
        }
    }
}

using MediatR;
using SIGTI.Application.Common.Exceptions;
using SIGTI.Application.Common.Interfaces.Persistence;

namespace SIGTI.Application.Features.Tickets.Queries.GetTicketById
{
    public sealed class GetTicketByIdHandler
        : IRequestHandler<GetTicketByIdQuery, GetTicketResponse>
    {
        private readonly ITicketRepository _ticketRepository;

        public GetTicketByIdHandler(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        public async Task<GetTicketResponse> Handle(
            GetTicketByIdQuery request,
            CancellationToken cancellationToken
        )
        {
            var ticket = await _ticketRepository.GetDetailsByIdAsync(
                request.TicketId,
                cancellationToken
            );

            if (ticket is null)
                throw new NotFoundException("Chamado não encontrado.");

            Console.WriteLine($"Department: {ticket.Department is null}");
            Console.WriteLine($"Queue: {ticket.Queue is null}");
            Console.WriteLine($"CreatedBy: {ticket.CreatedBy is null}");
            Console.WriteLine($"CurrentAssignment: {ticket.CurrentAssignment is null}");

            if (ticket.CurrentAssignment is not null)
            {
                Console.WriteLine($"Technician: {ticket.CurrentAssignment.Technician is null}");
            }

            return new GetTicketResponse(
                ticket.Id,
                ticket.Number,
                ticket.Title,
                ticket.Description,
                ticket.Status.ToString(),
                ticket.Priority.ToString(),
                ticket.Category.ToString(),
                ticket.Department.Name,
                ticket.Queue.Name,
                ticket.CurrentAssignment?.Technician.Name,
                ticket.CreatedBy.Name,
                ticket.CreatedAt
            );
        }
    }
}

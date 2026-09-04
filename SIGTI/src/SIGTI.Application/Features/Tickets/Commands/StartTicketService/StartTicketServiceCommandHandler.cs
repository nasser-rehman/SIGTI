using MediatR;
using SIGTI.Application.Common.Exceptions;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Domain.Entities;

namespace SIGTI.Application.Features.Tickets.Commands.StartTicketService
{
    public class StartTicketServiceCommandHandler
        : IRequestHandler<StartTicketServiceCommand>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IUnitOfWork _unitOfWork;

        public StartTicketServiceCommandHandler(
            ITicketRepository ticketRepository,
            IUnitOfWork unitOfWork
        )
        {
            _ticketRepository = ticketRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(
            StartTicketServiceCommand request,
            CancellationToken cancellationToken
        )
        {
            var ticket = await _ticketRepository.GetByIdAsync(
                request.TicketId,
                cancellationToken
            );

            if (ticket is null)
            {
                throw new NotFoundException(nameof(Ticket), request.TicketId);
            }

            ticket.StartService();

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}

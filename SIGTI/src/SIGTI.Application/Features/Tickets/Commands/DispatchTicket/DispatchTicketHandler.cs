using MediatR;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Application.Common.Interfaces.Services;
using SIGTI.Domain.Interfaces.Services;

namespace SIGTI.Application.Features.Tickets.Commands.DispatchTicket
{
    public sealed class DispatchTicketHandler : IRequestHandler<DispatchTicketCommand, DispatchTicketResponse>
    {
        private readonly IEntityReferenceService _referenceService;
        private readonly ITicketRepository _ticketRepository;
        private readonly ITicketAssignmentStrategy _assignmentStrategy;
        private readonly IUnitOfWork _unitOfWork;

        public Task<DispatchTicketResponse> Handle(DispatchTicketCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

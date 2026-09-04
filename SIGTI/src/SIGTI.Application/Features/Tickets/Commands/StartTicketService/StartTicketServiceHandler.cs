using MediatR;
using SIGTI.Application.Common.Exceptions;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Application.Common.Interfaces.Services;
using SIGTI.Domain.Entities;

namespace SIGTI.Application.Features.Tickets.Commands.StartTicketService
{
    public class StartTicketServiceHandler
        : IRequestHandler<StartTicketServiceCommand, StartTicketServiceResponse>
    {
        private readonly IEntityReferenceService _entityReferenceService;
        private readonly IUnitOfWork _unitOfWork;

        public StartTicketServiceHandler(
            IEntityReferenceService entityReferenceService,
            IUnitOfWork unitOfWork
        )
        {
            _entityReferenceService = entityReferenceService;
            _unitOfWork = unitOfWork;
        }

        public async Task<StartTicketServiceResponse> Handle(
            StartTicketServiceCommand request,
            CancellationToken cancellationToken
        )
        {
            var ticket = await _entityReferenceService.GetRequiredTicketAsync(
                request.TicketId,
                cancellationToken
            );

            ticket.StartService();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new StartTicketServiceResponse(
                ticket.Id,
                ticket.Status,
                ticket.UpdatedAt
            );
        }
    }
}

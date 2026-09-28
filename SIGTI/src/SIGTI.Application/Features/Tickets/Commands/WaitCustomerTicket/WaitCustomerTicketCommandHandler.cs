using MediatR;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Application.Common.Interfaces.Services;

namespace SIGTI.Application.Features.Tickets.Commands.WaitCustomerTicket
{
    public sealed class WaitCustomerTicketCommandHandler
        : IRequestHandler<WaitCustomerTicketCommand, WaitCustomerTicketResponse>
    {
        private readonly IEntityReferenceService _entityReferenceService;
        private readonly IUnitOfWork _unitOfWork;

        public WaitCustomerTicketCommandHandler(
            IEntityReferenceService entityReferenceService,
            IUnitOfWork unitOfWork
        )
        {
            _entityReferenceService = entityReferenceService;
            _unitOfWork = unitOfWork;
        }

        public async Task<WaitCustomerTicketResponse> Handle(
            WaitCustomerTicketCommand request,
            CancellationToken cancellationToken
        )
        {
            var ticket = await _entityReferenceService.GetRequiredTicketAsync(
                request.TicketId,
                cancellationToken
            );

            ticket.WaitCustomer();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new WaitCustomerTicketResponse(
                ticket.Id,
                ticket.Status,
                ticket.UpdatedAt
            );
        }
    }
}

using MediatR;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Application.Common.Interfaces.Services;

namespace SIGTI.Application.Features.Tickets.Commands.ReclassifyTicket
{
    public sealed class ReclassifyTicketCommandHandler
        : IRequestHandler<ReclassifyTicketCommand, ReclassifyTicketResponse>
    {
        private readonly IEntityReferenceService _entityReferenceService;
        private readonly IUnitOfWork _unitOfWork;

        public ReclassifyTicketCommandHandler(
            IEntityReferenceService entityReferenceService,
            IUnitOfWork unitOfWork
        )
        {
            _entityReferenceService = entityReferenceService;
            _unitOfWork = unitOfWork;
        }

        public async Task<ReclassifyTicketResponse> Handle(
            ReclassifyTicketCommand request,
            CancellationToken cancellationToken
        )
        {
            var ticket = await _entityReferenceService.GetRequiredTicketAsync(
                request.TicketId,
                cancellationToken
            );

            ticket.Reclassify(request.Priority, request.Category);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new ReclassifyTicketResponse(
                ticket.Id,
                ticket.Priority,
                ticket.Category,
                ticket.UpdatedAt
            );
        }
    }
}

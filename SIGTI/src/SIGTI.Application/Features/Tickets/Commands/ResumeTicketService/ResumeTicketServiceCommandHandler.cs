using MediatR;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Application.Common.Interfaces.Services;

namespace SIGTI.Application.Features.Tickets.Commands.ResumeTicketService
{
    public sealed class ResumeTicketServiceCommandHandler
        : IRequestHandler<
            ResumeTicketServiceCommand,
            ResumeTicketServiceResponse
        >
    {
        private readonly IEntityReferenceService _entityReferenceService;
        private readonly IUnitOfWork _unitOfWork;

        public ResumeTicketServiceCommandHandler(
            IEntityReferenceService entityReferenceService,
            IUnitOfWork unitOfWork
        )
        {
            _entityReferenceService = entityReferenceService;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResumeTicketServiceResponse> Handle(
            ResumeTicketServiceCommand request,
            CancellationToken cancellationToken
        )
        {
            var ticket = await _entityReferenceService.GetRequiredTicketAsync(
                request.TicketId,
                cancellationToken
            );

            ticket.ResumeService();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new ResumeTicketServiceResponse(
                ticket.Id,
                ticket.Status,
                ticket.UpdatedAt
            );
        }
    }
}

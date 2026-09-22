using MediatR;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Application.Common.Interfaces.Services;

namespace SIGTI.Application.Features.SupportQueues.Commands.RemoveMember
{
    public sealed class RemoveMemberCommandHandler
        : IRequestHandler<RemoveMemberCommand, RemoveMemberResponse>
    {
        private readonly IEntityReferenceService _entityReferenceService;
        private readonly IUnitOfWork _unitOfWork;

        public RemoveMemberCommandHandler(
            IEntityReferenceService entityReferenceService,
            IUnitOfWork unitOfWork
        )
        {
            _entityReferenceService = entityReferenceService;
            _unitOfWork = unitOfWork;
        }

        public async Task<RemoveMemberResponse> Handle(
            RemoveMemberCommand request,
            CancellationToken cancellationToken
        )
        {
            var queue = await _entityReferenceService.GetRequiredQueueAsync(
                request.QueueId,
                cancellationToken
            );

            var technician = await _entityReferenceService.GetRequiredUserAsync(
                request.TechnicianId,
                cancellationToken
            );

            queue.RemoveMember(technician);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new RemoveMemberResponse(queue.Id, technician.Id, false);
        }
    }
}

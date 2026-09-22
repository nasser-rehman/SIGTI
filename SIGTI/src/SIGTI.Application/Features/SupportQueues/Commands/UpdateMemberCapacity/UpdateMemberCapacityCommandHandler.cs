using MediatR;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Application.Common.Interfaces.Services;

namespace SIGTI.Application.Features.SupportQueues.Commands.UpdateMemberCapacity
{
    public sealed class UpdateMemberCapacityCommandHandler
        : IRequestHandler<
            UpdateMemberCapacityCommand,
            UpdateMemberCapacityResponse
        >
    {
        private readonly IEntityReferenceService _entityReferenceService;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateMemberCapacityCommandHandler(
            IEntityReferenceService entityReferenceService,
            IUnitOfWork unitOfWork
        )
        {
            _entityReferenceService = entityReferenceService;
            _unitOfWork = unitOfWork;
        }

        public async Task<UpdateMemberCapacityResponse> Handle(
            UpdateMemberCapacityCommand request,
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

            queue.UpdateMemberCapacity(
                technician,
                request.MaxConcurrentTickets
            );

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new UpdateMemberCapacityResponse(
                queue.Id,
                technician.Id,
                request.MaxConcurrentTickets,
                queue.UpdatedAt
            );
        }
    }
}

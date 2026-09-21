using MediatR;
using SIGTI.Application.Common.Exceptions;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Domain.Entities;

namespace SIGTI.Application.Features.SupportQueues.Commands.DeactivateSupportQueue
{
    public sealed class DeactivateSupportQueueCommandHandler
        : IRequestHandler<
            DeactivateSupportQueueCommand,
            DeactivateSupportQueueResponse
        >
    {
        private readonly ISupportQueueRepository _supportQueueRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeactivateSupportQueueCommandHandler(
            ISupportQueueRepository supportQueueRepository,
            IUnitOfWork unitOfWork
        )
        {
            _supportQueueRepository = supportQueueRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<DeactivateSupportQueueResponse> Handle(
            DeactivateSupportQueueCommand request,
            CancellationToken cancellationToken
        )
        {
            var queue = await _supportQueueRepository.GetByIdAsync(
                request.Id,
                cancellationToken
            );

            if (queue is null)
            {
                throw new NotFoundException(nameof(SupportQueue), request.Id);
            }

            queue.Deactivate();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new DeactivateSupportQueueResponse(
                queue.Id,
                queue.IsActive,
                queue.UpdatedAt
            );
        }
    }
}

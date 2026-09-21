using MediatR;
using SIGTI.Application.Common.Exceptions;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Domain.Entities;

namespace SIGTI.Application.Features.SupportQueues.Commands.ActivateSupportQueue
{
    public sealed class ActivateSupportQueueCommandHandler
        : IRequestHandler<
            ActivateSupportQueueCommand,
            ActivateSupportQueueResponse
        >
    {
        private readonly ISupportQueueRepository _supportQueueRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ActivateSupportQueueCommandHandler(
            ISupportQueueRepository supportQueueRepository,
            IUnitOfWork unitOfWork
        )
        {
            _supportQueueRepository = supportQueueRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ActivateSupportQueueResponse> Handle(
            ActivateSupportQueueCommand request,
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

            queue.Activate();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new ActivateSupportQueueResponse(
                queue.Id,
                queue.IsActive,
                queue.UpdatedAt
            );
        }
    }
}

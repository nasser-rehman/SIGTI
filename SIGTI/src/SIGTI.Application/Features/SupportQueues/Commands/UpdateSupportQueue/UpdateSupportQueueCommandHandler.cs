using MediatR;
using SIGTI.Application.Common.Exceptions;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Exceptions;

namespace SIGTI.Application.Features.SupportQueues.Commands.UpdateSupportQueue
{
    public sealed class UpdateSupportQueueCommandHandler
        : IRequestHandler<UpdateSupportQueueCommand, UpdateSupportQueueResponse>
    {
        private readonly ISupportQueueRepository _supportQueueRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateSupportQueueCommandHandler(
            ISupportQueueRepository supportQueueRepository,
            IUnitOfWork unitOfWork
        )
        {
            _supportQueueRepository = supportQueueRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<UpdateSupportQueueResponse> Handle(
            UpdateSupportQueueCommand request,
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

            if (
                !string.Equals(
                    queue.Name,
                    request.Name.Trim(),
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                var nameExists =
                    await _supportQueueRepository.ExistsByNameAsync(
                        request.Name.Trim(),
                        cancellationToken
                    );

                if (nameExists)
                {
                    throw new DomainException(
                        "Fila de suporte com o mesmo nome já existente."
                    );
                }
            }

            queue.UpdateName(request.Name);
            queue.UpdateDescription(request.Description);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new UpdateSupportQueueResponse(
                queue.Id,
                queue.Name,
                queue.Description,
                queue.IsActive,
                queue.UpdatedAt
            );
        }
    }
}

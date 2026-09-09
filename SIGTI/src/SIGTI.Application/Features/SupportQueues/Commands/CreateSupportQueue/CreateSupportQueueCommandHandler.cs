using MediatR;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Exceptions;

namespace SIGTI.Application.Features.SupportQueues.Commands.CreateSupportQueue
{
    public sealed class CreateSupportQueueCommandHandler
        : IRequestHandler<CreateSupportQueueCommand, CreateSupportQueueResponse>
    {
        private readonly ISupportQueueRepository _supportQueueRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateSupportQueueCommandHandler(
            ISupportQueueRepository supportQueueRepository,
            IUnitOfWork unitOfWork
        )
        {
            _supportQueueRepository = supportQueueRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateSupportQueueResponse> Handle(
            CreateSupportQueueCommand request,
            CancellationToken cancellationToken
        )
        {
            if (
                await _supportQueueRepository.ExistsByNameAsync(
                    request.Name,
                    cancellationToken
                )
            )
            {
                throw new DomainException(
                    $"Já existe uma fila de suporte com o nome '{request.Name}'"
                );
            }

            var queue = new SupportQueue(request.Name, request.Description);

            await _supportQueueRepository.AddAsync(queue, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CreateSupportQueueResponse(
                queue.Id,
                queue.Name,
                queue.Description
            );
        }
    }
}

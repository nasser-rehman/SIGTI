using MediatR;
using SIGTI.Application.Common.Interfaces.Persistence;

namespace SIGTI.Application.Features.SupportQueues.Queries.ListActiveSupportQueues
{
    public sealed class ListActiveSupportQueuesQueryHandler
        : IRequestHandler<
            ListActiveSupportQueuesQuery,
            IReadOnlyCollection<ListActiveSupportQueuesResponse>
        >
    {
        private readonly ISupportQueueRepository _supportQueueRepository;

        public ListActiveSupportQueuesQueryHandler(
            ISupportQueueRepository supportQueueRepository
        )
        {
            _supportQueueRepository = supportQueueRepository;
        }

        public async Task<
            IReadOnlyCollection<ListActiveSupportQueuesResponse>
        > Handle(
            ListActiveSupportQueuesQuery request,
            CancellationToken cancellationToken
        )
        {
            var activeQueues = request.IncludeInactive
                ? await _supportQueueRepository.GetAllAsync(cancellationToken)
                : await _supportQueueRepository.ListActiveAsync(
                    cancellationToken
                );

            var response = activeQueues
                .Select(queue => new ListActiveSupportQueuesResponse(
                    queue.Id,
                    queue.Name,
                    queue.Description,
                    queue.IsActive
                ))
                .ToList();

            return response;
        }
    }
}

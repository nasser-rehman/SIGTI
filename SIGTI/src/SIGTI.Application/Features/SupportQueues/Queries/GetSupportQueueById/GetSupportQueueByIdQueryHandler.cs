using MediatR;
using SIGTI.Application.Common.Exceptions;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Domain.Entities;

namespace SIGTI.Application.Features.SupportQueues.Queries.GetSupportQueueById
{
    public sealed class GetSupportQueueByIdQueryHandler
        : IRequestHandler<GetSupportQueueByIdQuery, GetSupportQueueByIdResponse>
    {
        private readonly ISupportQueueRepository _supportQueueRepository;

        public GetSupportQueueByIdQueryHandler(
            ISupportQueueRepository supportQueueRepository
        )
        {
            _supportQueueRepository = supportQueueRepository;
        }

        public async Task<GetSupportQueueByIdResponse> Handle(
            GetSupportQueueByIdQuery request,
            CancellationToken cancellationToken
        )
        {
            var queue = await _supportQueueRepository.GetByIdAsync(
                request.Id,
                cancellationToken
            );

            if (queue is null)
                throw new NotFoundException(nameof(SupportQueue), request.Id);

            var members = queue
                .Members.Select(m => new SupportQueueMemberResponse(
                    m.TechnicianId,
                    m.Technician?.Name ?? string.Empty,
                    m.Technician?.Email?.Value ?? string.Empty,
                    m.MaxConcurrentTickets,
                    m.IsActive,
                    m.JoinedAt,
                    m.LeftAt
                ))
                .ToList();

            return new GetSupportQueueByIdResponse(
                queue.Id,
                queue.Name,
                queue.Description,
                queue.IsActive,
                queue.CreatedAt,
                queue.UpdatedAt,
                members
            );
        }
    }
}

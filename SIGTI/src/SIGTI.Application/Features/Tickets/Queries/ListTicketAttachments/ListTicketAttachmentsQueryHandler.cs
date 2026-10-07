using MediatR;
using SIGTI.Application.Common.Interfaces.Services;

namespace SIGTI.Application.Features.Tickets.Queries.ListTicketAttachments
{
    public sealed class ListTicketAttachmentsQueryHandler
        : IRequestHandler<
            ListTicketAttachmentsQuery,
            IReadOnlyList<ListTicketAttachmentsResponse>
        >
    {
        private readonly IEntityReferenceService _entityReferenceService;

        public ListTicketAttachmentsQueryHandler(
            IEntityReferenceService entityReferenceService
        )
        {
            _entityReferenceService = entityReferenceService;
        }

        public async Task<IReadOnlyList<ListTicketAttachmentsResponse>> Handle(
            ListTicketAttachmentsQuery request,
            CancellationToken cancellationToken
        )
        {
            var ticket = await _entityReferenceService.GetRequiredTicketAsync(
                request.TicketId,
                cancellationToken
            );

            return ticket
                .Attachments.OrderBy(a => a.CreatedAt)
                .Select(a => new ListTicketAttachmentsResponse(
                    a.Id,
                    a.TicketId,
                    a.FileName,
                    a.ContentType,
                    a.FileSize,
                    a.UploadedById,
                    a.UploadedBy?.Name ?? string.Empty,
                    a.CreatedAt
                ))
                .ToList()
                .AsReadOnly();
        }
    }
}

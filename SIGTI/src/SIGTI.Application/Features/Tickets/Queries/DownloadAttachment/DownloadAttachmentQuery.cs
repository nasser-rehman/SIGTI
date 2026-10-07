using MediatR;

namespace SIGTI.Application.Features.Tickets.Queries.DownloadAttachment
{
    public sealed record DownloadAttachmentQuery(
        Guid TicketId,
        Guid AttachmentId
    ) : IRequest<DownloadAttachmentResponse>;
}

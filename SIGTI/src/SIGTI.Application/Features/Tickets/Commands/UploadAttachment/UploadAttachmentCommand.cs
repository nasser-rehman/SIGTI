using MediatR;

namespace SIGTI.Application.Features.Tickets.Commands.UploadAttachment
{
    public sealed record UploadAttachmentCommand(
        Guid TicketId,
        string FileName,
        string ContentType,
        long FileSize,
        Stream ContentStream,
        Guid UploadedById
    ) : IRequest<UploadAttachmentResponse>;
}

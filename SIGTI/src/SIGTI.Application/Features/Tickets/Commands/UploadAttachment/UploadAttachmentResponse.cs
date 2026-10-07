namespace SIGTI.Application.Features.Tickets.Commands.UploadAttachment
{
    public sealed record UploadAttachmentResponse(
        Guid Id,
        Guid TicketId,
        string FileName,
        string ContentType,
        long FileSize,
        string StorageKey,
        Guid UploadedById,
        string UploadedByName,
        DateTime CreatedAt
    );
}

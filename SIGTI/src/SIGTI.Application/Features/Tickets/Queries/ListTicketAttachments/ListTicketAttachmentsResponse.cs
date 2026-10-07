namespace SIGTI.Application.Features.Tickets.Queries.ListTicketAttachments
{
    public sealed record ListTicketAttachmentsResponse(
        Guid Id,
        Guid TicketId,
        string FileName,
        string ContentType,
        long FileSize,
        Guid UploadedById,
        string uploadedByName,
        DateTime CreatedAt
    );
}

namespace SIGTI.Application.Features.Tickets.Queries.DownloadAttachment
{
    public sealed record DownloadAttachmentResponse(
        string FileName,
        string ContentType,
        long FileSize,
        Stream Stream
    );
}

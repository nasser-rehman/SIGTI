using MediatR;
using SIGTI.Application.Common.Exceptions;
using SIGTI.Application.Common.Interfaces.Services;

namespace SIGTI.Application.Features.Tickets.Queries.DownloadAttachment
{
    public sealed class DownloadAttachmentQueryHandler
        : IRequestHandler<DownloadAttachmentQuery, DownloadAttachmentResponse>
    {
        private readonly IEntityReferenceService _entityReferenceService;
        private readonly IFileStorageService _fileStorageService;

        public DownloadAttachmentQueryHandler(
            IEntityReferenceService entityReferenceService,
            IFileStorageService fileStorageService
        )
        {
            _entityReferenceService = entityReferenceService;
            _fileStorageService = fileStorageService;
        }

        public async Task<DownloadAttachmentResponse> Handle(
            DownloadAttachmentQuery request,
            CancellationToken cancellationToken
        )
        {
            var ticket = await _entityReferenceService.GetRequiredTicketAsync(
                request.TicketId,
                cancellationToken
            );

            var attachment = ticket.Attachments.FirstOrDefault(a =>
                a.Id == request.AttachmentId
            );

            if (attachment is null)
                throw new NotFoundException(
                    $"Anexo com ID '{request.AttachmentId}' não encontrado para este ticket"
                );

            var exists = await _fileStorageService.ExistsAsync(
                attachment.StorageKey,
                cancellationToken
            );

            if (!exists)
                throw new NotFoundException(
                    "O arquivo físico não foi encontrado no armazenamento."
                );

            var stream = await _fileStorageService.DownloadAsync(
                attachment.StorageKey,
                cancellationToken
            );

            return new DownloadAttachmentResponse(
                attachment.FileName,
                attachment.ContentType,
                attachment.FileSize,
                stream
            );
        }
    }
}

using MediatR;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Application.Common.Interfaces.Services;
using SIGTI.Domain.Entities;

namespace SIGTI.Application.Features.Tickets.Commands.UploadAttachment
{
    public sealed class UploadAttachmentCommandHandler
        : IRequestHandler<UploadAttachmentCommand, UploadAttachmentResponse>
    {
        private readonly IEntityReferenceService _entityReferenceService;
        private readonly IFileStorageService _fileStorageService;
        private readonly IUnitOfWork _unitOfWork;

        public UploadAttachmentCommandHandler(
            IEntityReferenceService entityReferenceService,
            IFileStorageService fileStorageService,
            IUnitOfWork unitOfWork
        )
        {
            _entityReferenceService = entityReferenceService;
            _fileStorageService = fileStorageService;
            _unitOfWork = unitOfWork;
        }

        public async Task<UploadAttachmentResponse> Handle(
            UploadAttachmentCommand request,
            CancellationToken cancellationToken
        )
        {
            var ticket = await _entityReferenceService.GetRequiredTicketAsync(
                request.TicketId,
                cancellationToken
            );

            var uploadedBy = await _entityReferenceService.GetRequiredUserAsync(
                request.UploadedById,
                cancellationToken
            );

            var extension = Path.GetExtension(request.FileName)
                .ToLowerInvariant();
            var storageKey =
                $"tickets/{ticket.Id:D}/{Guid.NewGuid():N}{extension}";

            var attachment = new Attachment(
                request.FileName,
                request.ContentType,
                request.FileSize,
                storageKey,
                ticket,
                uploadedBy
            );

            ticket.AddAttachment(attachment);

            await _fileStorageService.UploadAsync(
                request.ContentStream,
                storageKey,
                request.ContentType,
                cancellationToken
            );

            try
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch
            {
                await _fileStorageService.DeleteAsync(
                    storageKey,
                    CancellationToken.None
                );
                throw;
            }

            return new UploadAttachmentResponse(
                attachment.Id,
                ticket.Id,
                attachment.FileName,
                attachment.ContentType,
                attachment.FileSize,
                attachment.StorageKey,
                uploadedBy.Id,
                uploadedBy.Name,
                attachment.CreatedAt
            );
        }
    }
}

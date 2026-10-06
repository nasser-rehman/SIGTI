using SIGTI.Domain.Common;
using SIGTI.Domain.Exceptions;

namespace SIGTI.Domain.Entities
{
    public sealed class Attachment : BaseEntity
    {
        private const int MaxFileNameLength = 255;
        private const int MaxContentTypeLength = 100;
        private const int MaxStorageKeyLength = 500;

        public string FileName { get; private set; } = null!;
        public string ContentType { get; private set; } = null!;
        public long FileSize { get; private set; }
        public string StorageKey { get; private set; } = null!;

        public Guid TicketId { get; private set; }
        public Ticket Ticket { get; private set; } = null!;

        public Guid UploadedById { get; private set; }
        public User UploadedBy { get; private set; } = null!;

        private Attachment() { }

        public Attachment(
            string fileName,
            string contentType,
            long fileSize,
            string storageKey,
            Ticket ticket,
            User uploadedBy
        )
        {
            if (ticket is null)
                throw new DomainException("O ticket é obrigatório.");

            if (uploadedBy is null)
                throw new DomainException(
                    "O usuário que enviou o anexo é obrigatório."
                );

            if (string.IsNullOrWhiteSpace(fileName))
                throw new DomainException("O nome do arquivo é obrigatório.");

            fileName = fileName.Trim();
            if (fileName.Length > MaxFileNameLength)
                throw new DomainException(
                    $"O nome do arquivo não pode exceder {MaxFileNameLength} caracteres."
                );

            if (string.IsNullOrWhiteSpace(contentType))
                throw new DomainException(
                    "O tipo de conteúdo (Content-Type) é obrigatório."
                );

            contentType = contentType.Trim();
            if (contentType.Length > MaxContentTypeLength)
                throw new DomainException(
                    $"O tipo de conteúdo não pode exceder {MaxContentTypeLength} caracteres."
                );

            if (fileSize <= 0)
                throw new DomainException(
                    "O tamanho do arquivo deve ser maior que zero."
                );

            if (string.IsNullOrWhiteSpace(storageKey))
                throw new DomainException(
                    "A chave de armazenamento é obrigatória."
                );

            storageKey = storageKey.Trim();
            if (storageKey.Length > MaxStorageKeyLength)
                throw new DomainException(
                    $"A chave de armazenamento não pode exceder {MaxStorageKeyLength} caracteres."
                );

            FileName = fileName;
            ContentType = contentType;
            FileSize = fileSize;
            StorageKey = storageKey;

            Ticket = ticket;
            TicketId = ticket.Id;

            UploadedBy = uploadedBy;
            UploadedById = uploadedBy.Id;
        }
    }
}

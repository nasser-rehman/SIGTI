using SIGTI.Domain.Entities;

namespace SIGTI.Domain.Tests.Builders
{
    public class AttachmentBuilder
    {
        private string _fileName = "evidencia_erro.png";
        private string _contentType = "image/png";
        private long _fileSize = 1024 * 50;
        private string _storageKey = "tickets/test-ticket-id/test-file.png";
        private Ticket? _ticket;
        private User? _uploadedBy;

        public AttachmentBuilder WithFileName(string fileName)
        {
            _fileName = fileName;
            return this;
        }

        public AttachmentBuilder WithContentType(string contentType)
        {
            _contentType = contentType;
            return this;
        }

        public AttachmentBuilder WithFileSize(long fileSize)
        {
            _fileSize = fileSize;
            return this;
        }

        public AttachmentBuilder WithStorageKey(string storageKey)
        {
            _storageKey = storageKey;
            return this;
        }

        public AttachmentBuilder WithTicket(Ticket ticket)
        {
            _ticket = ticket;
            return this;
        }

        public AttachmentBuilder WithUploadedBy(User uploadedBy)
        {
            _uploadedBy = uploadedBy;
            return this;
        }

        public Attachment Build()
        {
            var ticket = _ticket ?? new TicketBuilder().Build();
            var uploadedBy = _uploadedBy ?? new UserBuilder().Build();

            return new Attachment(
                _fileName,
                _contentType,
                _fileSize,
                _storageKey,
                ticket,
                uploadedBy
            );
        }
    }
}

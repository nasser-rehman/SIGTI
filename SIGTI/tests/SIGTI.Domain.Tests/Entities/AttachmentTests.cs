using FluentAssertions;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Exceptions;
using SIGTI.Domain.Tests.Builders;
using Xunit;

namespace SIGTI.Domain.Tests.Entities
{
    public class AttachmentTests
    {
        [Fact]
        public void Should_Create_Attachment_Successfully()
        {
            var attachment = new AttachmentBuilder().Build();

            attachment.FileName.Should().Be("evidencia_erro.png");
            attachment.ContentType.Should().Be("image/png");
            attachment.FileSize.Should().Be(50 * 1024);
            attachment
                .StorageKey.Should()
                .Be("tickets/test-ticket-id/test-file.png");
            attachment.Ticket.Should().NotBeNull();
            attachment.UploadedBy.Should().NotBeNull();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Should_Throw_When_FileName_Is_Null_Or_Whitespace(
            string? invalidFileName
        )
        {
            Action act = () =>
                new AttachmentBuilder().WithFileName(invalidFileName!).Build();

            act.Should()
                .Throw<DomainException>()
                .WithMessage("O nome do arquivo é obrigatório.");
        }

        [Fact]
        public void Should_Throw_When_FileName_Exceeds_Max_Length()
        {
            var longFileName = new string('a', 256);

            Action act = () =>
                new AttachmentBuilder().WithFileName(longFileName).Build();

            act.Should()
                .Throw<DomainException>()
                .WithMessage(
                    "O nome do arquivo não pode exceder 255 caracteres."
                );
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Should_Throw_When_ContentType_Is_Null_Or_Whitespace(
            string? invalidContentType
        )
        {
            Action act = () =>
                new AttachmentBuilder()
                    .WithContentType(invalidContentType!)
                    .Build();

            act.Should()
                .Throw<DomainException>()
                .WithMessage(
                    "O tipo de conteúdo (Content-Type) é obrigatório."
                );
        }

        [Fact]
        public void Should_Throw_When_ContentType_Exceeds_Max_Length()
        {
            var longContentType = new string('a', 101);

            Action act = () =>
                new AttachmentBuilder()
                    .WithContentType(longContentType)
                    .Build();

            act.Should()
                .Throw<DomainException>()
                .WithMessage(
                    "O tipo de conteúdo não pode exceder 100 caracteres."
                );
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-1024)]
        public void Should_Throw_When_FileSize_Is_Zero_Or_Negative(
            long invalidFileSize
        )
        {
            Action act = () =>
                new AttachmentBuilder().WithFileSize(invalidFileSize).Build();

            act.Should()
                .Throw<DomainException>()
                .WithMessage("O tamanho do arquivo deve ser maior que zero.");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Should_Throw_When_StorageKey_Is_Null_Or_Whitespace(
            string? invalidStorageKey
        )
        {
            Action act = () =>
                new AttachmentBuilder()
                    .WithStorageKey(invalidStorageKey!)
                    .Build();

            act.Should()
                .Throw<DomainException>()
                .WithMessage("A chave de armazenamento é obrigatória.");
        }

        [Fact]
        public void Should_Throw_When_StorageKey_Exceeds_Max_Length()
        {
            var longKey = new string('a', 501);

            Action act = () =>
                new AttachmentBuilder().WithStorageKey(longKey).Build();

            act.Should()
                .Throw<DomainException>()
                .WithMessage(
                    "A chave de armazenamento não pode exceder 500 caracteres."
                );
        }

        [Fact]
        public void Should_Throw_When_Ticket_Is_Null()
        {
            Action act = () =>
                new Attachment(
                    "arquivo.png",
                    "image/png",
                    100,
                    "key",
                    null!,
                    new UserBuilder().Build()
                );

            act.Should()
                .Throw<DomainException>()
                .WithMessage("O ticket é obrigatório.");
        }

        [Fact]
        public void Should_Throw_When_UploadedBy_Is_Null()
        {
            Action act = () =>
                new Attachment(
                    "arquivo.png",
                    "image/png",
                    100,
                    "key",
                    new TicketBuilder().Build(),
                    null!
                );

            act.Should()
                .Throw<DomainException>()
                .WithMessage("O usuário que enviou o anexo é obrigatório.");
        }
    }
}

using FluentValidation;
using SIGTI.Application.Common.Interfaces.Services;

namespace SIGTI.Application.Features.Tickets.Commands.UploadAttachment
{
    public sealed class UploadAttachmentCommandValidator
        : AbstractValidator<UploadAttachmentCommand>
    {
        private const long MaxFileSizeInBytes = 10 * 1024 * 1024;

        public UploadAttachmentCommandValidator(
            IFileSecurityValidator fileSecurityValidator
        )
        {
            RuleFor(x => x.TicketId)
                .NotEmpty()
                .WithMessage("O identificador do ticket é obrigatório.");

            RuleFor(x => x.UploadedById)
                .NotEmpty()
                .WithMessage(
                    "O identificador do usuário que enviou o anexo é obrigatório."
                );

            RuleFor(x => x.FileName)
                .NotEmpty()
                .WithMessage("O nome do arquivo é obrigatório.")
                .MaximumLength(255)
                .WithMessage(
                    "O nome do arquivo não pode exceder 255 caracteres."
                )
                .Must(fileSecurityValidator.IsAllowedExtension)
                .WithMessage("A extensão do arquivo não é permitida.");

            RuleFor(x => x.ContentType)
                .NotEmpty()
                .WithMessage("O tipo de conteúdo (Content-Type) é obrigatório.")
                .MaximumLength(100)
                .WithMessage(
                    "O tipo de conteúdo não pode exceder 100 caracteres."
                )
                .Must(fileSecurityValidator.IsAllowedContentType)
                .WithMessage(
                    "O tipo de conteúdo (Content-Type) informado não é permitido."
                );

            RuleFor(x => x.FileSize)
                .GreaterThan(0)
                .WithMessage("O tamanho do arquivo deve ser maior que zero.")
                .LessThanOrEqualTo(MaxFileSizeInBytes)
                .WithMessage("O tamanho do arquivo não pode exceder 10 MB.");

            RuleFor(x => x.ContentStream)
                .NotNull()
                .WithMessage(
                    "O fluxo de dados (stream) do arquivo é obrigatório."
                );

            RuleFor(x => x)
                .MustAsync(
                    async (cmd, cancellationToken) =>
                    {
                        if (
                            cmd.ContentStream is null
                            || string.IsNullOrWhiteSpace(cmd.FileName)
                        )
                            return true;

                        return await fileSecurityValidator.ValidateFileSignatureAsync(
                            cmd.ContentStream,
                            cmd.FileName,
                            cancellationToken
                        );
                    }
                )
                .WithMessage(
                    "A assinatura binária do arquivo (Magic Bytes) não corresponde à sua extensão (arquivo suspeito ou corrompido)."
                );
        }
    }
}

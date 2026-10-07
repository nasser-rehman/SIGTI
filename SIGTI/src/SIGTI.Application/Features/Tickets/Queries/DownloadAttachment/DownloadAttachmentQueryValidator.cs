using FluentValidation;

namespace SIGTI.Application.Features.Tickets.Queries.DownloadAttachment
{
    public sealed class DownloadAttachmentQueryValidator
        : AbstractValidator<DownloadAttachmentQuery>
    {
        public DownloadAttachmentQueryValidator()
        {
            RuleFor(x => x.TicketId)
                .NotEmpty()
                .WithMessage("O identificador do ticket é obrigatório.");

            RuleFor(x => x.AttachmentId)
                .NotEmpty()
                .WithMessage("O identificador do anexo é obrigatório.");
        }
    }
}

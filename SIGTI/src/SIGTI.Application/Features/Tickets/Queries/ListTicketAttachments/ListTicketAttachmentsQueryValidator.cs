using FluentValidation;

namespace SIGTI.Application.Features.Tickets.Queries.ListTicketAttachments;

public sealed class ListTicketAttachmentsQueryValidator
    : AbstractValidator<ListTicketAttachmentsQuery>
{
    public ListTicketAttachmentsQueryValidator()
    {
        RuleFor(x => x.TicketId)
            .NotEmpty()
            .WithMessage("O identificador do ticket é obrigatório.");
    }
}

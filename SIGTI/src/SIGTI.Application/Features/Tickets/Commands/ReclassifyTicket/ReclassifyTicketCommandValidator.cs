using FluentValidation;

namespace SIGTI.Application.Features.Tickets.Commands.ReclassifyTicket
{
    public sealed class ReclassifyTicketCommandValidator
        : AbstractValidator<ReclassifyTicketCommand>
    {
        public ReclassifyTicketCommandValidator()
        {
            RuleFor(x => x.TicketId)
                .NotEmpty()
                .WithMessage("O identificador do ticket é obrigatório.");

            RuleFor(x => x.Priority)
                .IsInEnum()
                .WithMessage("Prioridade do ticket inválida.");

            RuleFor(x => x.Category)
                .IsInEnum()
                .WithMessage("Categoria do ticket inválida.");
        }
    }
}

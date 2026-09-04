using FluentValidation;

namespace SIGTI.Application.Features.Tickets.Commands.DispatchTicket
{
    public sealed class DispatchTicketValidator
        : AbstractValidator<DispatchTicketCommand>
    {
        public DispatchTicketValidator()
        {
            RuleFor(x => x.TicketId)
                .NotEmpty()
                .WithMessage("O identificador do  ticket é obrigatório.");

            RuleFor(x => x.AssignedById)
                .NotEmpty()
                .WithMessage(
                    "O identificador do usuário que realizou a atribuição é obrigatório."
                );

            RuleFor(x => x.Reason)
                .MaximumLength(500)
                .WithMessage(
                    "A razão da atribuição não pode ultrapassar 500 caracteres."
                )
                .When(x => !string.IsNullOrWhiteSpace(x.Reason));
        }
    }
}

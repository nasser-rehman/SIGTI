using FluentValidation;

namespace SIGTI.Application.Features.Tickets.Commands.StartTicketService
{
    public class StartTicketServiceValidator
        : AbstractValidator<StartTicketServiceCommand>
    {
        public StartTicketServiceValidator()
        {
            RuleFor(x => x.TicketId)
                .NotEmpty()
                .WithMessage("O identificador do ticket é obrigatório.");
        }
    }
}

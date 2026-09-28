using FluentValidation;

namespace SIGTI.Application.Features.Tickets.Commands.ResumeTicketService
{
    public sealed class ResumeTicketServiceCommandValidator
        : AbstractValidator<ResumeTicketServiceCommand>
    {
        public ResumeTicketServiceCommandValidator()
        {
            RuleFor(x => x.TicketId)
                .NotEmpty()
                .WithMessage("O identificador do ticket é obrigatório.");
        }
    }
}

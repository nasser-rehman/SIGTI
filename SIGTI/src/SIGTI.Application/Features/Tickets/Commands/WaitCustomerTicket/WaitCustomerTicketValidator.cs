using FluentValidation;

namespace SIGTI.Application.Features.Tickets.Commands.WaitCustomerTicket
{
    public sealed class WaitCustomerTicketValidator
        : AbstractValidator<WaitCustomerTicketCommand>
    {
        public WaitCustomerTicketValidator()
        {
            RuleFor(x => x.TicketId)
                .NotEmpty()
                .WithMessage("O identificador do ticket é obrigatório.");
        }
    }
}

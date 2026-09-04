using FluentValidation;

namespace SIGTI.Application.Features.Tickets.Commands.StartTicketService
{
    public class StartTicketServiceCommandValidator
        : AbstractValidator<StartTicketServiceCommand>
    {
        public StartTicketServiceCommandValidator()
        {
            RuleFor(x => x.TicketId)
                .NotEmpty()
                .WithMessage("TicketId is required.");
        }
    }
}

using FluentValidation;

namespace SIGTI.Application.Features.Tickets.Commands.CreateTicket;

public sealed class CreateTicketValidator
    : AbstractValidator<CreateTicketCommand>
{
    public CreateTicketValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(5000);

        RuleFor(x => x.DepartmentId)
            .NotEqual(Guid.Empty);

        RuleFor(x => x.QueueId)
            .NotEqual(Guid.Empty);

        RuleFor(x => x.CreatedById)
            .NotEqual(Guid.Empty);
    }
}

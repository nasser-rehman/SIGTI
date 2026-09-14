using FluentValidation;

namespace SIGTI.Application.Features.Tickets.Queries.GetTicketTimeline
{
    public sealed class GetTicketTimelineQueryValidator
        : AbstractValidator<GetTicketTimelineQuery>
    {
        public GetTicketTimelineQueryValidator()
        {
            RuleFor(x => x.TicketId)
                .NotEmpty()
                .WithMessage("O identificador do ticket é obrigatório.");
        }
    }
}

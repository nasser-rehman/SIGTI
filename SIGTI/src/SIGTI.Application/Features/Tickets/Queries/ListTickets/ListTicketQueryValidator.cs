using FluentValidation;

namespace SIGTI.Application.Features.Tickets.Queries.ListTickets
{
    public sealed class ListTicketQueryValidator : AbstractValidator<ListTicketsQuery>
    {
        public ListTicketQueryValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1)
                .WithMessage("A página deve ser maior ou igual a 1.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100)
                .WithMessage("O tamanho da página deve estar entre 1 e 100.");
        }
    }
}

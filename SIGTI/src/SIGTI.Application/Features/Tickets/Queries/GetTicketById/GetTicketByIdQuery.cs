using MediatR;

namespace SIGTI.Application.Features.Tickets.Queries.GetTicketById
{
    public sealed record GetTicketByIdQuery(Guid TicketId) : IRequest<GetTicketResponse>;
}

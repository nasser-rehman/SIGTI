using MediatR;

namespace SIGTI.Application.Features.Tickets.Commands.StartTicketService
{
    public sealed record StartTicketServiceCommand(Guid TicketId)
        : IRequest<StartTicketServiceResponse>;
}

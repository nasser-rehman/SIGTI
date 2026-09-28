using MediatR;

namespace SIGTI.Application.Features.Tickets.Commands.WaitCustomerTicket
{
    public sealed record WaitCustomerTicketCommand(Guid TicketId)
        : IRequest<WaitCustomerTicketResponse>;
}

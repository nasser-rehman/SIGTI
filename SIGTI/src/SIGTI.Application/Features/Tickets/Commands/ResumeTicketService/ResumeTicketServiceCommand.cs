using MediatR;

namespace SIGTI.Application.Features.Tickets.Commands.ResumeTicketService
{
    public sealed record ResumeTicketServiceCommand(Guid TicketId)
        : IRequest<ResumeTicketServiceResponse>;
}

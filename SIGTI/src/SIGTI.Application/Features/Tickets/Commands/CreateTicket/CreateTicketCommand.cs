using MediatR;
using SIGTI.Domain.Enums;

namespace SIGTI.Application.Features.Tickets.Commands.CreateTicket;

public sealed record CreateTicketCommand(
    string Title,
    string Description,
    TicketPriority Priority,
    TicketCategory Category,
    Guid DepartmentId,
    Guid QueueId,
    Guid CreatedById
) : IRequest<CreateTicketResponse>;

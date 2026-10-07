using MediatR;

namespace SIGTI.Application.Features.Tickets.Queries.ListTicketAttachments
{
    public sealed record ListTicketAttachmentsQuery(Guid TicketId)
        : IRequest<IReadOnlyList<ListTicketAttachmentsResponse>>;
}

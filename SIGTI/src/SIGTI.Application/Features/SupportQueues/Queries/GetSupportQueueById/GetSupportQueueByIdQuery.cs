using MediatR;

namespace SIGTI.Application.Features.SupportQueues.Queries.GetSupportQueueById
{
    public sealed record GetSupportQueueByIdQuery(Guid Id)
        : IRequest<GetSupportQueueByIdResponse>;
}

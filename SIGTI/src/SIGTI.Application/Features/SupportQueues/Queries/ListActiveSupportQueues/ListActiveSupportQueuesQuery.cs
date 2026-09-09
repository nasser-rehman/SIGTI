using MediatR;

namespace SIGTI.Application.Features.SupportQueues.Queries.ListActiveSupportQueues
{
    public sealed record ListActiveSupportQueuesQuery
        : IRequest<IReadOnlyCollection<ListActiveSupportQueuesResponse>>;
}

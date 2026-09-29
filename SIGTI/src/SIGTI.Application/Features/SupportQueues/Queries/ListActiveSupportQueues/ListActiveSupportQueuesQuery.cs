using MediatR;

namespace SIGTI.Application.Features.SupportQueues.Queries.ListActiveSupportQueues
{
    public sealed record ListActiveSupportQueuesQuery(
        bool IncludeInactive = false
    ) : IRequest<IReadOnlyCollection<ListActiveSupportQueuesResponse>>;
}

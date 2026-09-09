namespace SIGTI.Application.Features.SupportQueues.Queries.ListActiveSupportQueues
{
    public sealed record ListActiveSupportQueuesResponse(
        Guid Id,
        string Name,
        string Description
    );
}

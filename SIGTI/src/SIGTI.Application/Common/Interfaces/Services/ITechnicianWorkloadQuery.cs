namespace SIGTI.Application.Common.Interfaces.Services
{
    public interface ITechnicianWorkloadQuery
    {
        Task<IReadOnlyDictionary<Guid, int>> GetCurrentWorkloadAsync(
            IEnumerable<Guid> technicianIds,
            CancellationToken cancellationToken
        );
    }
}

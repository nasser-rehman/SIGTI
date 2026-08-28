using Microsoft.EntityFrameworkCore;
using SIGTI.Application.Common.Interfaces.Services;
using SIGTI.Infrastructure.Persistence.Context;

namespace SIGTI.Infrastructure.Persistence.Queries
{
    public sealed class TechnicianWorkloadQuery : ITechnicianWorkloadQuery
    {
        private readonly ApplicationDbContext _context;

        public TechnicianWorkloadQuery(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyDictionary<Guid, int>> GetCurrentWorkloadAsync(
            IEnumerable<Guid> technicianIds,
            CancellationToken cancellationToken
        )
        {
            var workloads = await _context
                .TicketAssignments.Where(x =>
                    technicianIds.Contains(x.TechnicianId) && x.FinishedAt == null
                )
                .GroupBy(x => x.TechnicianId)
                .Select(g => new { TechnicianId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.TechnicianId, x => x.Count, cancellationToken);

            var result = technicianIds.ToDictionary(
                id => id,
                id => workloads.TryGetValue(id, out var count) ? count : 0
            );

            return result;
        }
    }
}

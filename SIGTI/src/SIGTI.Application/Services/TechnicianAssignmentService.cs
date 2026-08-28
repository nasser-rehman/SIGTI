using SIGTI.Application.Common.Interfaces.Services;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Exceptions;

namespace SIGTI.Application.Services
{
    public sealed class TechnicianAssignmentService : ITechnicianAssignmentService
    {
        private readonly ITechnicianWorkloadQuery _workloadQuery;

        public TechnicianAssignmentService(ITechnicianWorkloadQuery workloadQuery)
        {
            _workloadQuery = workloadQuery;
        }

        public async Task<SupportQueueMember> SelectTechnicianAsync(
            SupportQueue queue,
            CancellationToken cancellationToken
        )
        {
            var members = queue.Members.Where(x => x.IsActive).ToList();

            if (!members.Any())
                throw new DomainException("Não há técnicos ativos na fila.");

            var technicianIds = members.Select(x => x.TechnicianId).ToList();

            var workloads = await _workloadQuery.GetCurrentWorkloadAsync(
                technicianIds,
                cancellationToken
            );

            var availableMembers = members.Where(member =>
            {
                var current = workloads.GetValueOrDefault(member.TechnicianId);
                return current < member.MaxConcurrentTickets;
            });

            var selected = availableMembers
                .OrderBy(member => workloads.GetValueOrDefault(member.TechnicianId))
                .ThenBy(member => member.JoinedAt)
                .FirstOrDefault();

            if (selected is null)
                throw new DomainException("Nenhum técnico disponível para receber novos chamados.");

            return selected;
        }
    }
}

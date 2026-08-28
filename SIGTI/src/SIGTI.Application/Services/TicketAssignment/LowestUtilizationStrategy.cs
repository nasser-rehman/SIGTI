using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Enums;
using SIGTI.Domain.Exceptions;
using SIGTI.Domain.Interfaces.Services;

namespace SIGTI.Application.Services.TicketAssignment
{
    public class LowestUtilizationStrategy : ITicketAssignmentStrategy
    {
        public SupportQueueMember SelectTechnician(SupportQueue queue, IReadOnlyCollection<Ticket> activeTickets)
        {
            var activeMembers = queue.Members.Where(m => m.IsActive).ToList();

            if (!activeMembers.Any())
                throw new DomainException("A fila não possui técnicos ativos.");

            var workloads = activeMembers.Select(member =>
            {
                var currentTickets = activeTickets.Count(ticket => ticket.CurrentAssignment?.TechnicianId == member.TechnicianId);
                var utilization = CalculateUtilization(currentTickets, member.MaxConcurrentTickets);

                return new
                {
                    Member = member,
                    CurrentTickets = currentTickets,
                    Utilization = utilization
                };
            });

            var availableMembers = workloads.Where(x => x.CurrentTickets < x.Member.MaxConcurrentTickets).ToList();

            if (!availableMembers.Any())
                throw new DomainException("Nenhum técnico disponível na fila");

            var selected = availableMembers.OrderBy(x => x.Utilization).ThenBy(x => x.Member.JoinedAt).First();

            return selected.Member;
        }

        private static double CalculateUtilization(int currentTickets, int MaxConcurrentTickets)
        {
            return (double)currentTickets / MaxConcurrentTickets;
        }

    }
}

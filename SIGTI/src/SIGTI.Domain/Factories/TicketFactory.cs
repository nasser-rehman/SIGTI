using SIGTI.Domain.Entities;
using SIGTI.Domain.Enums;

namespace SIGTI.Domain.Factories
{
    public sealed class TicketFactory
    {
        public Ticket Create(
            int number,
            string title,
            string description,
            TicketPriority priority,
            TicketCategory category,
            Department department,
            User createdBy,
            SupportQueue queue
        )
        {
            return new Ticket(
                number,
                title,
                description,
                priority,
                category,
                department,
                createdBy,
                queue
            );
        }
    }
}

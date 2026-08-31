using SIGTI.Domain.Entities;
using SIGTI.Domain.Enums;
using SIGTI.Domain.ValueObjects;

namespace SIGTI.Domain.Tests.Builders
{
    public class TicketBuilder
    {
        private static int _ticketCounter = 0;
        private int _number = Interlocked.Increment(ref _ticketCounter);
        private string _title = "Erro no computador";
        private string _description = "Tela azul ao iniciar.";
        private TicketPriority _priority = TicketPriority.Medium;
        private TicketCategory _category = TicketCategory.Hardware;
        private Department _department = new DepartmentBuilder().Build();
        private TicketStatus _status = TicketStatus.New;
        private User _createdBy = new UserBuilder().Build();
        private SupportQueue _queue = new SupportQueueBuilder().Build();

        public TicketBuilder WithNumber(int number)
        {
            _number = number;
            return this;
        }

        public TicketBuilder WithTitle(string title)
        {
            _title = title;
            return this;
        }

        public TicketBuilder WithDescription(string description)
        {
            _description = description;
            return this;
        }

        public TicketBuilder WithPriority(TicketPriority priority)
        {
            _priority = priority;
            return this;
        }

        public TicketBuilder WithCategory(TicketCategory category)
        {
            _category = category;
            return this;
        }

        public TicketBuilder WithDepartment(Department department)
        {
            _department = department;
            return this;
        }

        public TicketBuilder WaitQueue()
        {
            _status = TicketStatus.WaitingQueue;
            return this;
        }

        public TicketBuilder StartService()
        {
            _status = TicketStatus.InProgress;
            return this;
        }

        public TicketBuilder WaitCustomer()
        {
            _status = TicketStatus.WaitingCustomer;
            return this;
        }

        public TicketBuilder Resolver()
        {
            _status = TicketStatus.Resolved;
            return this;
        }

        public TicketBuilder Close()
        {
            _status = TicketStatus.Closed;
            return this;
        }

        public TicketBuilder WithQueue(SupportQueue queue)
        {
            _queue = queue;
            return this;
        }

        public TicketBuilder WithCreatedBy(User createdBy)
        {
            _createdBy = createdBy;
            return this;
        }

        public Ticket Build()
        {
            return new Ticket(
                _number,
                _title,
                _description,
                _priority,
                _category,
                _department,
                _createdBy,
                _queue
            );
        }

        public Ticket BuildWithoutQueue()
        {
            return new Ticket(
                _number,
                _title,
                _description,
                _priority,
                _category,
                _department,
                _createdBy,
                null!
            );
        }

        public Ticket BuildAsResolved()
        {
            var ticket = Build();
            var technician = new UserBuilder()
                .WithRole(Role.Technician)
                .Build();
            var assigner = new UserBuilder()
                .WithRole(Role.Administrator)
                .Build();
            ticket.SendToQueue();
            ticket.AssignTechnician(
                technician,
                assigner,
                "Atribuição inicial ao N1"
            );
            ticket.StartService();
            ticket.Resolve();

            return ticket;
        }

        public Ticket BuildAsClosed()
        {
            var ticket = BuildAsResolved();
            ticket.Close();

            return ticket;
        }
    }
}

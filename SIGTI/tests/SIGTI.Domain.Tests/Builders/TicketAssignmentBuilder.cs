using System;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Enums;

namespace SIGTI.Domain.Tests.Builders
{
    public class TicketAssignmentBuilder
    {
        private Guid _ticketId = Guid.NewGuid();
        private Guid _technicianId = Guid.NewGuid();
        private Guid _assignedById = Guid.NewGuid();
        private string _reason = "Atribuição inicial ao N1";
        private bool _isFinished = false;

        private Ticket? _ticket = null;
        private User? _technician = null;
        private User? _assignedBy = null;

        public TicketAssignmentBuilder WithTicket(Ticket ticket)
        {
            _ticket = ticket;
            _ticketId = ticket.Id;
            return this;
        }

        public TicketAssignmentBuilder WithTechnician(User technician)
        {
            _technician = technician;
            _technicianId = technician.Id;
            return this;
        }

        public TicketAssignmentBuilder WithTechnicianId(Guid technicianId)
        {
            _technicianId = technicianId;
            return this;
        }

        public TicketAssignmentBuilder WithAssignedById(Guid assignedById)
        {
            _assignedById = assignedById;
            return this;
        }

        public TicketAssignmentBuilder WithReason(string reason)
        {
            _reason = reason;
            return this;
        }

        public TicketAssignmentBuilder AsFinished()
        {
            _isFinished = true;
            return this;
        }

        public TicketAssignment Build()
        {
            var ticket = _ticket ?? new TicketBuilder().Build();
            var technician = _technician ?? new UserBuilder().WithRole(Role.Technician).Build();
            var assignedBy = _assignedBy ?? new UserBuilder().WithRole(Role.Administrator).Build();

            var assignment = new TicketAssignment(ticket, technician, assignedBy, _reason);

            if (_isFinished)
                assignment.MarkAsFinished();

            return assignment;
        }
    }
}

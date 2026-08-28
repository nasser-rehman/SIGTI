using Moq;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Application.Common.Interfaces.Services;
using SIGTI.Application.Features.Tickets.Commands.CreateTicket;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Enums;
using SIGTI.Domain.Factories;
using SIGTI.Domain.Tests.Builders;

namespace SIGTI.Application.Tests.Fixtures
{
    public sealed class CreateTicketHandlerFixture
    {
        public Mock<ITicketRepository> TicketRepository { get; } = new();
        public Mock<ITicketNumberGenerator> TicketNumberGenerator { get; } = new();
        public Mock<IUnitOfWork> UnitOfWork { get; } = new();
        public Mock<IEntityReferenceService> EntityReferenceService { get; } = new();
        public Mock<ITechnicianAssignmentService> AssignmentService { get; } = new();

        public Department Department { get; }
        public SupportQueue Queue { get; } 
        public User CreatedBy { get; } 
        public User Technician { get; }
        public User SystemUser { get; }
        public SupportQueueMember SelectedMember { get;  }

        public int TicketNumber { get; } = 10;

        public CreateTicketCommand Command { get; }

        public CreateTicketHandler Handler { get; }

        public CreateTicketHandlerFixture()
        {
            Department = new DepartmentBuilder().Build();
            Queue = new SupportQueueBuilder().Build();
            CreatedBy = new UserBuilder().Build();


            Technician = new UserBuilder()
                .WithRole(Role.Technician)
                .Build();

            SystemUser = new UserBuilder()
                .WithRole(Role.Administrator)
                .Build();

            SelectedMember = new SupportQueueMember(Queue, Technician, 10);

            Command = new CreateTicketCommand(
                "Erro no computador",
                "Tela azul ao iniciar.",
                TicketPriority.Medium,
                TicketCategory.Hardware,
                Department.Id,
                Queue.Id,
                CreatedBy.Id);

            ConfigureDefaults();

            Handler = new CreateTicketHandler(
                TicketRepository.Object,
                TicketNumberGenerator.Object,
                UnitOfWork.Object,
                new TicketFactory(),
                EntityReferenceService.Object,
                AssignmentService.Object);
        }

        private void ConfigureDefaults() 
        {
            EntityReferenceService
                .Setup(x => x.GetRequiredUserAsync(
                    Command.CreatedById,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(CreatedBy);

            EntityReferenceService
                .Setup(x => x.GetRequiredDepartmentAsync(
                    Command.DepartmentId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Department);

            EntityReferenceService
                .Setup(x => x.GetRequiredQueueAsync(
                    Command.QueueId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Queue);

            EntityReferenceService
                .Setup(x => x.GetRequiredSystemUserAsync(
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(SystemUser);

            AssignmentService
                .Setup(x => x.SelectTechnicianAsync(
                    Queue,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(SelectedMember);

            TicketNumberGenerator
                .Setup(x => x.GetNextAsync(
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(TicketNumber);
        }
    }
 }

using FluentAssertions;
using Moq;
using SIGTI.Application.Common.Exceptions;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Application.Features.Tickets.Commands.StartTicketService;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Enums;
using SIGTI.Domain.Tests.Builders;
using Xunit;

namespace SIGTI.Application.Tests.Features.Tickets.Commands.StartTicketService
{
    public class StartTicketServiceCommandHandlerTests
    {
        private readonly Mock<ITicketRepository> _ticketRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly StartTicketServiceCommandHandler _handler;

        public StartTicketServiceCommandHandlerTests()
        {
            _ticketRepositoryMock = new Mock<ITicketRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new StartTicketServiceCommandHandler(
                _ticketRepositoryMock.Object,
                _unitOfWorkMock.Object
            );
        }

        [Fact]
        public async Task Handle_WhenTicketExistsAndIsAssigned_ShouldTransitionToInProgressAndCommit()
        {
            // Arrange
            var department = new DepartmentBuilder().Build();
            var queue = new SupportQueueBuilder().Build();
            var creator = new UserBuilder().WithDepartment(department).Build();
            var technician = new UserBuilder()
                .WithDepartment(department)
                .Build();

            var ticket = new TicketBuilder()
                .WithDepartment(department)
                .WithQueue(queue)
                .WithCreatedBy(creator)
                .Build();

            ticket.AssignTechnician(technician, creator, "Initial assignment");

            _ticketRepositoryMock
                .Setup(r =>
                    r.GetByIdAsync(ticket.Id, It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(ticket);

            var command = new StartTicketServiceCommand(ticket.Id);

            //Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            ticket.Status.Should().Be(TicketStatus.InProgress);

            _unitOfWorkMock.Verify(
                uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task Handle_WhenTicketDoesNotExist_ShouldThrowNotFoundException()
        {
            // Arrange
            var nonExistentTicketId = Guid.NewGuid();

            _ticketRepositoryMock
                .Setup(r =>
                    r.GetByIdAsync(
                        nonExistentTicketId,
                        It.IsAny<CancellationToken>()
                    )
                )
                .ReturnsAsync((Ticket?)null);

            var command = new StartTicketServiceCommand(nonExistentTicketId);

            // Act
            var act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();

            _unitOfWorkMock.Verify(
                uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }
    }
}

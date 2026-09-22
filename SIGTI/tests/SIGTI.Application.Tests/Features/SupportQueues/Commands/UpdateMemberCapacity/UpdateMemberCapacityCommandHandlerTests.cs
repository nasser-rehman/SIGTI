using FluentAssertions;
using Moq;
using SIGTI.Application.Common.Exceptions;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Application.Common.Interfaces.Services;
using SIGTI.Application.Features.SupportQueues.Commands.UpdateMemberCapacity;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Enums;
using SIGTI.Domain.Exceptions;
using SIGTI.Domain.Tests.Builders;

namespace SIGTI.Application.Tests.Features.SupportQueues.Commands.UpdateMemberCapacity
{
    public class UpdateMemberCapacityCommandHandlerTests
    {
        private readonly Mock<IEntityReferenceService> _entityReferenceServiceMock;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly UpdateMemberCapacityCommandHandler _handler;

        public UpdateMemberCapacityCommandHandlerTests()
        {
            _entityReferenceServiceMock = new Mock<IEntityReferenceService>();
            _unitOfWork = new Mock<IUnitOfWork>();
            _handler = new UpdateMemberCapacityCommandHandler(
                _entityReferenceServiceMock.Object,
                _unitOfWork.Object
            );
        }

        [Fact]
        public async Task Handle_WhenMemberExists_ShouldUpdateCapacityAndCommit()
        {
            // Arrange
            var queue = new SupportQueueBuilder().Build();
            var technician = new UserBuilder()
                .WithRole(Role.Technician)
                .Build();
            queue.AddMember(technician, 3);

            _entityReferenceServiceMock
                .Setup(s =>
                    s.GetRequiredQueueAsync(
                        queue.Id,
                        It.IsAny<CancellationToken>()
                    )
                )
                .ReturnsAsync(queue);

            _entityReferenceServiceMock
                .Setup(s =>
                    s.GetRequiredUserAsync(
                        technician.Id,
                        It.IsAny<CancellationToken>()
                    )
                )
                .ReturnsAsync(technician);

            var command = new UpdateMemberCapacityCommand(
                queue.Id,
                technician.Id,
                8
            );

            // Act
            var response = await _handler.Handle(
                command,
                CancellationToken.None
            );

            // Assert
            response.Should().NotBeNull();
            response.QueueId.Should().Be(queue.Id);
            response.TechnicianId.Should().Be(technician.Id);
            response.MaxConcurrentTickets.Should().Be(8);
            response.UpdatedAt.Should().NotBeNull();

            var member = queue.Members.First();
            member.MaxConcurrentTickets.Should().Be(8);

            _unitOfWork.Verify(
                uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task Handle_WhenQueueNotFound_ShouldThrowNotFoundException()
        {
            // Arrange
            var nonExistentQueueId = Guid.NewGuid();
            var technicianId = Guid.NewGuid();

            _entityReferenceServiceMock
                .Setup(s =>
                    s.GetRequiredQueueAsync(
                        nonExistentQueueId,
                        It.IsAny<CancellationToken>()
                    )
                )
                .ThrowsAsync(
                    new NotFoundException(
                        nameof(SupportQueue),
                        nonExistentQueueId
                    )
                );

            var command = new UpdateMemberCapacityCommand(
                nonExistentQueueId,
                technicianId,
                5
            );

            // Act
            var act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_WhenTechnicianNotFound_ShouldThrowNotFoundException()
        {
            // Arrange
            var queue = new SupportQueueBuilder().Build();
            var nonExistentTechId = Guid.NewGuid();

            _entityReferenceServiceMock
                .Setup(s =>
                    s.GetRequiredQueueAsync(
                        queue.Id,
                        It.IsAny<CancellationToken>()
                    )
                )
                .ReturnsAsync(queue);

            _entityReferenceServiceMock
                .Setup(s =>
                    s.GetRequiredUserAsync(
                        nonExistentTechId,
                        It.IsAny<CancellationToken>()
                    )
                )
                .ThrowsAsync(
                    new NotFoundException(nameof(User), nonExistentTechId)
                );

            var command = new UpdateMemberCapacityCommand(
                queue.Id,
                nonExistentTechId,
                5
            );

            // Act
            var act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_WhenTechnicianIsNotActiveMember_ShouldThrowDomainException()
        {
            // Arrange
            var queue = new SupportQueueBuilder().Build();
            var technician = new UserBuilder()
                .WithRole(Role.Technician)
                .Build();

            _entityReferenceServiceMock
                .Setup(s =>
                    s.GetRequiredQueueAsync(
                        queue.Id,
                        It.IsAny<CancellationToken>()
                    )
                )
                .ReturnsAsync(queue);

            _entityReferenceServiceMock
                .Setup(s =>
                    s.GetRequiredUserAsync(
                        technician.Id,
                        It.IsAny<CancellationToken>()
                    )
                )
                .ReturnsAsync(technician);

            var command = new UpdateMemberCapacityCommand(
                queue.Id,
                technician.Id,
                5
            );

            // Act
            var act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<DomainException>()
                .WithMessage("O técnico não é membro ativo da fila.");
        }
    }
}

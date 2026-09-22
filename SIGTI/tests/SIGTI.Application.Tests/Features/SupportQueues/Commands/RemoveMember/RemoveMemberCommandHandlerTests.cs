using FluentAssertions;
using Moq;
using SIGTI.Application.Common.Exceptions;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Application.Common.Interfaces.Services;
using SIGTI.Application.Features.SupportQueues.Commands.RemoveMember;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Enums;
using SIGTI.Domain.Exceptions;
using SIGTI.Domain.Tests.Builders;

namespace SIGTI.Application.Tests.Features.SupportQueues.Commands.RemoveMember
{
    public class RemoveMemberCommandHandlerTests
    {
        private readonly Mock<IEntityReferenceService> _entityReferenceServiceMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly RemoveMemberCommandHandler _handler;

        public RemoveMemberCommandHandlerTests()
        {
            _entityReferenceServiceMock = new Mock<IEntityReferenceService>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new RemoveMemberCommandHandler(
                _entityReferenceServiceMock.Object,
                _unitOfWorkMock.Object
            );
        }

        [Fact]
        public async Task Handle_WhenMemberExistsInQueue_ShouldRemoveMemberAndCommit()
        {
            // Arrange
            var queue = new SupportQueueBuilder().Build();
            var technician = new UserBuilder()
                .WithRole(Role.Technician)
                .Build();

            queue.AddMember(technician, 5);

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

            var command = new RemoveMemberCommand(queue.Id, technician.Id);

            // Act
            var response = await _handler.Handle(
                command,
                CancellationToken.None
            );

            // Assert
            response.Should().NotBeNull();
            response.QueueId.Should().Be(queue.Id);
            response.TechnicianId.Should().Be(technician.Id);
            response.IsActive.Should().BeFalse();

            queue.GetActiveMemberCount().Should().Be(0);

            _unitOfWorkMock.Verify(
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

            var command = new RemoveMemberCommand(
                nonExistentQueueId,
                technicianId
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

            var command = new RemoveMemberCommand(queue.Id, nonExistentTechId);

            // Act
            var act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_WhenTechnicianIsNotMemberOfQueue_ShouldThrowDomainException()
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

            var command = new RemoveMemberCommand(queue.Id, technician.Id);

            // Act
            var act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<DomainException>()
                .WithMessage("O técnico não é membro da fila.");
        }
    }
}

using FluentAssertions;
using Moq;
using SIGTI.Application.Common.Exceptions;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Application.Features.SupportQueues.Commands.ActivateSupportQueue;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Tests.Builders;
using Xunit;

namespace SIGTI.Application.Tests.Features.SupportQueues.Commands.ActivateSupportQueue
{
    public class ActivateSupportQueueCommandHandlerTests
    {
        private readonly Mock<ISupportQueueRepository> _supportQueueRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly ActivateSupportQueueCommandHandler _handler;

        public ActivateSupportQueueCommandHandlerTests()
        {
            _supportQueueRepositoryMock = new Mock<ISupportQueueRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new ActivateSupportQueueCommandHandler(
                _supportQueueRepositoryMock.Object,
                _unitOfWorkMock.Object
            );
        }

        [Fact]
        public async Task Handle_WhenQueueExists_ShouldActivateAndReturnResponse()
        {
            // Arrange
            var queue = new SupportQueueBuilder().Build();
            queue.Deactivate();

            var command = new ActivateSupportQueueCommand(queue.Id);

            _supportQueueRepositoryMock
                .Setup(r =>
                    r.GetByIdAsync(queue.Id, It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(queue);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(queue.Id);
            result.IsActive.Should().BeTrue();
            result.UpdatedAt.Should().NotBeNull();

            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task Handle_WhenQueueDoesNotExist_ShouldThrowNotFoundException()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();
            var command = new ActivateSupportQueueCommand(nonExistentId);

            _supportQueueRepositoryMock
                .Setup(r =>
                    r.GetByIdAsync(nonExistentId, It.IsAny<CancellationToken>())
                )
                .ReturnsAsync((SupportQueue?)null);

            // Act
            var act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}

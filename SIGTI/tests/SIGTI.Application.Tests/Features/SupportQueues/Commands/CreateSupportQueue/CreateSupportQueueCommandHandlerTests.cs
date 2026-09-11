using FluentAssertions;
using Moq;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Application.Features.SupportQueues.Commands.CreateSupportQueue;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Exceptions;
using SIGTI.Domain.Tests.Builders;

namespace SIGTI.Application.Tests.Features.SupportQueues.Commands.CreateSupportQueue
{
    public class CreateSupportQueueCommandHandlerTests
    {
        private readonly Mock<ISupportQueueRepository> _supportQueueRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly CreateSupportQueueCommandHandler _handler;

        public CreateSupportQueueCommandHandlerTests()
        {
            _supportQueueRepositoryMock = new Mock<ISupportQueueRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _handler = new CreateSupportQueueCommandHandler(
                _supportQueueRepositoryMock.Object,
                _unitOfWorkMock.Object
            );
        }

        [Fact]
        public async Task Handle_WhenDataIsValid_ShouldCreateQueueAndCommit()
        {
            // Arrange
            var queue = new SupportQueueBuilder()
                .WithName("Green Queue")
                .Build();

            _supportQueueRepositoryMock
                .Setup(q =>
                    q.ExistsByNameAsync(
                        queue.Name,
                        It.IsAny<CancellationToken>()
                    )
                )
                .ReturnsAsync(false);

            var command = new CreateSupportQueueCommand(
                queue.Name,
                queue.Description
            );

            // Act
            var response = await _handler.Handle(
                command,
                CancellationToken.None
            );

            // Assert
            response.Should().NotBeNull();
            response.Id.Should().NotBeEmpty();
            response.Name.Should().Be(command.Name);
            response.Description.Should().Be(command.Description);

            _supportQueueRepositoryMock.Verify(
                repo =>
                    repo.AddAsync(
                        It.Is<SupportQueue>(q =>
                            q.Name == command.Name
                            && q.Description == command.Description
                        ),
                        It.IsAny<CancellationToken>()
                    ),
                Times.Once
            );

            _unitOfWorkMock.Verify(
                uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task Handle_WhenQueueNameAlreadyExists_ShouldThrowDomainException()
        {
            // Arrange
            var queue = new SupportQueueBuilder().Build();

            _supportQueueRepositoryMock
                .Setup(x =>
                    x.ExistsByNameAsync(
                        queue.Name,
                        It.IsAny<CancellationToken>()
                    )
                )
                .ReturnsAsync(true);

            var command = new CreateSupportQueueCommand(
                queue.Name,
                queue.Description
            );

            // Act
            var act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainException>();

            _supportQueueRepositoryMock.Verify(
                repo =>
                    repo.AddAsync(
                        It.IsAny<SupportQueue>(),
                        It.IsAny<CancellationToken>()
                    ),
                Times.Never
            );
            _unitOfWorkMock.Verify(
                uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }
    }
}

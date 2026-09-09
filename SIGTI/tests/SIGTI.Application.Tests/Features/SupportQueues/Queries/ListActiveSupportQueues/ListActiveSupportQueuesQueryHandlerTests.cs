using FluentAssertions;
using Moq;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Application.Features.SupportQueues.Queries.ListActiveSupportQueues;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Tests.Builders;

namespace SIGTI.Application.Tests.Features.SupportQueues.Queries.ListActiveSupportQueues
{
    public class ListActiveSupportQueuesQueryHandlerTests
    {
        private readonly Mock<ISupportQueueRepository> _supportQueueRepositoryMock;
        private readonly ListActiveSupportQueuesQueryHandler _handler;

        public ListActiveSupportQueuesQueryHandlerTests()
        {
            _supportQueueRepositoryMock = new Mock<ISupportQueueRepository>();
            _handler = new ListActiveSupportQueuesQueryHandler(
                _supportQueueRepositoryMock.Object
            );
        }

        [Fact]
        public async Task Handle_WhenActiveQueueExists_ShouldReturnMappedQueues()
        {
            // Arrange
            var queueOne = new SupportQueueBuilder()
                .WithName("Queue N1")
                .Build();
            var queueTwo = new SupportQueueBuilder()
                .WithName("Queue N2")
                .Build();

            List<SupportQueue> queues = new();
            queues.Add(queueOne);
            queues.Add(queueTwo);

            _supportQueueRepositoryMock
                .Setup(x => x.ListActiveAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(queues);

            // Act
            var response = await _handler.Handle(
                new ListActiveSupportQueuesQuery(),
                CancellationToken.None
            );

            // Assert
            response.Should().NotBeNull();
            response.Should().HaveCount(2);
            response
                .Should()
                .Contain(queue =>
                    queue.Id == queueOne.Id && queue.Name == queueOne.Name
                );
        }

        [Fact]
        public async Task Handle_WhenNoActiveQueuesExist_ShouldReturnEmptyCollection()
        {
            List<SupportQueue> queues = new();

            _supportQueueRepositoryMock
                .Setup(x => x.ListActiveAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(queues);

            var response = await _handler.Handle(
                new ListActiveSupportQueuesQuery(),
                CancellationToken.None
            );

            response.Should().NotBeNull();
            response.Should().BeEmpty();
        }
    }
}

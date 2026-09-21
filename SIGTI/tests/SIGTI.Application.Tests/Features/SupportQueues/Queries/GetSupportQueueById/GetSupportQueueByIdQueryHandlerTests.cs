using FluentAssertions;
using Moq;
using SIGTI.Application.Common.Exceptions;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Application.Features.SupportQueues.Queries.GetSupportQueueById;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Enums;
using SIGTI.Domain.Tests.Builders;

namespace SIGTI.Application.Tests.Features.SupportQueues.Queries.GetSupportQueueById
{
    public class GetSupportQueueByIdQueryHandlerTests
    {
        private readonly Mock<ISupportQueueRepository> _supportQueueRepositoryMock;
        private readonly GetSupportQueueByIdQueryHandler _handler;

        public GetSupportQueueByIdQueryHandlerTests()
        {
            _supportQueueRepositoryMock = new Mock<ISupportQueueRepository>();
            _handler = new GetSupportQueueByIdQueryHandler(
                _supportQueueRepositoryMock.Object
            );
        }

        [Fact]
        public async Task Handle_WhenQueueExistsWithMembers_ShouldReturnQueueDetailsAndMembers()
        {
            // Arrange
            var queue = new SupportQueueBuilder().Build();
            var technician = new UserBuilder()
                .WithRole(Role.Technician)
                .Build();

            queue.AddMember(technician, 5);

            _supportQueueRepositoryMock
                .Setup(r =>
                    r.GetByIdAsync(queue.Id, It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(queue);

            var query = new GetSupportQueueByIdQuery(queue.Id);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(queue.Id);
            result.Name.Should().Be(queue.Name);
            result.Description.Should().Be(queue.Description);
            result.IsActive.Should().BeTrue();
            result.Members.Should().HaveCount(1);

            var memberResult = result.Members.First();
            memberResult.TechnicianId.Should().Be(technician.Id);
            memberResult.TechnicianName.Should().Be(technician.Name);
            memberResult.Email.Should().Be(technician.Email.Value);
            memberResult.MaxConcurrentTickets.Should().Be(5);
            memberResult.IsActive.Should().BeTrue();

            _supportQueueRepositoryMock.Verify(
                r => r.GetByIdAsync(queue.Id, It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task Handle_WhenQueueDoesNotExist_ShouldThrowNotFoundException()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            _supportQueueRepositoryMock
                .Setup(r =>
                    r.GetByIdAsync(nonExistentId, It.IsAny<CancellationToken>())
                )
                .ReturnsAsync((SupportQueue?)null);

            var query = new GetSupportQueueByIdQuery(nonExistentId);

            // Act
            var act = () => _handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}

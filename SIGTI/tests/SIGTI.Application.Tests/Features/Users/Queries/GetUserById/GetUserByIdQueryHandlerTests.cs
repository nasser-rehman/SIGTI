using FluentAssertions;
using Moq;
using SIGTI.Application.Common.Exceptions;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Application.Features.Users.Queries.GetUserById;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Enums;
using SIGTI.Domain.Tests.Builders;

namespace SIGTI.Application.Tests.Features.Users.Queries.GetUserById
{
    public class GetUserByIdQueryHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly GetUserByIdQueryHandler _handler;

        public GetUserByIdQueryHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _handler = new GetUserByIdQueryHandler(_userRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenUserExists_ShouldReturnUserDetails()
        {
            // Arrange
            var user = new UserBuilder()
                .WithName("User Name Test")
                .WithEmail("user@sigti.local")
                .WithRole(Role.Technician)
                .Build();

            _userRepositoryMock
                .Setup(r =>
                    r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(user);

            var query = new GetUserByIdQuery(user.Id);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(user.Id);
            result.Name.Should().Be(user.Name);
            result.Email.Should().Be(user.Email.Value);
            result.Role.Should().Be(Role.Technician);
            result.DepartmentId.Should().Be(user.DepartmentId);
            result.IsActive.Should().BeTrue();
            result.CreatedAt.Should().Be(user.CreatedAt);

            _userRepositoryMock.Verify(
                r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task Handle_WhenUserDoesNotExist_ShouldThrowNotFoundException()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            _userRepositoryMock
                .Setup(r =>
                    r.GetByIdAsync(nonExistentId, It.IsAny<CancellationToken>())
                )
                .ReturnsAsync((User?)null);

            var query = new GetUserByIdQuery(nonExistentId);

            // Act
            var act = () => _handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();

            _userRepositoryMock.Verify(
                r =>
                    r.GetByIdAsync(
                        nonExistentId,
                        It.IsAny<CancellationToken>()
                    ),
                Times.Once
            );
        }
    }
}

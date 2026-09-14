using FluentAssertions;
using Moq;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Application.Features.Users.Queries.ListUsers;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Enums;
using SIGTI.Domain.Tests.Builders;

namespace SIGTI.Application.Tests.Features.Users.Queries.ListUsers
{
    public class ListUsersQueryHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly ListUsersQueryHandler _handler;

        public ListUsersQueryHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _handler = new ListUsersQueryHandler(_userRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WhenRoleFilterIsProvided_ShouldReturnUsersFilteredByRole()
        {
            // Arrange
            var technician = new UserBuilder()
                .WithName("Técnico Teste")
                .WithEmail("tecnico@sigti.local")
                .WithRole(Role.Technician)
                .Build();

            _userRepositoryMock
                .Setup(r =>
                    r.ListByRoleAsync(
                        Role.Technician,
                        It.IsAny<CancellationToken>()
                    )
                )
                .ReturnsAsync(new List<User> { technician });

            var query = new ListUsersQuery(Role.Technician);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(1);
            var response = result.First();
            response.Id.Should().Be(technician.Id);
            response.Name.Should().Be(technician.Name);
            response.Email.Should().Be(technician.Email.Value);
            response.Role.Should().Be(technician.Role);
            response.DepartmentId.Should().Be(technician.DepartmentId);
            response.IsActive.Should().Be(technician.IsActive);

            _userRepositoryMock.Verify(
                r =>
                    r.ListByRoleAsync(
                        Role.Technician,
                        It.IsAny<CancellationToken>()
                    ),
                Times.Once
            );
            _userRepositoryMock.Verify(
                r => r.ListAllAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task Handle_WhenNoRoleFilterProvided_ShouldReturnAllUsers()
        {
            // Arrange
            var userOne = new UserBuilder()
                .WithName("Administrador")
                .WithRole(Role.Administrator)
                .Build();

            var userTwo = new UserBuilder()
                .WithName("Technician")
                .WithRole(Role.Technician)
                .Build();

            _userRepositoryMock
                .Setup(s => s.ListAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<User> { userOne, userTwo });

            var query = new ListUsersQuery(null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().HaveCount(2);
            _userRepositoryMock.Verify(
                v => v.ListAllAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );
            _userRepositoryMock.Verify(
                v =>
                    v.ListByRoleAsync(
                        It.IsAny<Role>(),
                        It.IsAny<CancellationToken>()
                    ),
                Times.Never
            );
        }
    }
}

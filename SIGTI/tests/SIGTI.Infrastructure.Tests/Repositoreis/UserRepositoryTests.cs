using FluentAssertions;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Enums;
using SIGTI.Domain.Tests.Builders;
using SIGTI.Domain.ValueObjects;
using SIGTI.Infrastructure.Persistence.Repositories;
using SIGTI.Infrastructure.Tests.Fixtures;

namespace SIGTI.Infrastructure.Tests.Repositories
{
    [Collection("DatabaseCollection")]
    public class UserRepositoryTests : IAsyncLifetime
    {
        private readonly PostgreSqlDatabaseFixture _fixture;

        public UserRepositoryTests(PostgreSqlDatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        public async Task InitializeAsync()
        {
            // Reset database state before each test execution
            await _fixture.ResetDatabaseAsync();
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task GetByIdAsync_WhenUserExists_ShouldReturnUserWIthDepartmentLoaded()
        {
            // Arrange
            var department = new DepartmentBuilder().Build();
            var user = new UserBuilder()
                .WithDepartment(department)
                .WithRole(Role.Technician)
                .WithEmail("tech@sigti.local")
                .Build();

            await using (var setupContext = _fixture.CreateDbContext())
            {
                await setupContext.Departments.AddAsync(department);
                await setupContext.Users.AddAsync(user);
                await setupContext.SaveChangesAsync();
            }

            // Act
            User? result;
            await using (var actContext = _fixture.CreateDbContext())
            {
                var repository = new UserRepository(actContext);
                result = await repository.GetByIdAsync(
                    user.Id,
                    CancellationToken.None
                );
            }

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(user.Id);
            result.Email.Should().Be(user.Email);
            result.Name.Should().Be(user.Name);
        }

        [Fact]
        public async Task GetByIdAsync_WhenUserDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            var nonExistentUserId = Guid.NewGuid();

            // Act
            User? result;
            await using (var actContext = _fixture.CreateDbContext())
            {
                var repository = new UserRepository(actContext);
                result = await repository.GetByIdAsync(
                    nonExistentUserId,
                    CancellationToken.None
                );
            }

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByEmailAsync_WhenEmailMatches_ShouldReturnCorrespondingUser()
        {
            // Arrange
            var department = new DepartmentBuilder().Build();
            Email targetEmail = new Email("admin.security@sigti.local");
            var user = new UserBuilder()
                .WithDepartment(department)
                .WithEmail(targetEmail)
                .Build();

            await using (var setupContext = _fixture.CreateDbContext())
            {
                await setupContext.Departments.AddAsync(department);
                await setupContext.Users.AddAsync(user);
                await setupContext.SaveChangesAsync();
            }

            // Act
            User? result;
            await using (var actContext = _fixture.CreateDbContext())
            {
                var repository = new UserRepository(actContext);
                result = await repository.GetByEmailAsync(
                    targetEmail,
                    CancellationToken.None
                );
            }

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(user.Id);
            result.Email.Should().Be(targetEmail);
        }

        [Fact]
        public async Task ExistsByEmailAsync_WhenEmailExists_ShouldReturnTrue()
        {
            // Arrange
            var department = new DepartmentBuilder().Build();
            Email existingEmail = new Email("technician.lead@sigti.local");
            var user = new UserBuilder()
                .WithDepartment(department)
                .WithEmail(existingEmail)
                .Build();

            await using (var setupContext = _fixture.CreateDbContext())
            {
                await setupContext.Departments.AddAsync(department);
                await setupContext.Users.AddAsync(user);
                await setupContext.SaveChangesAsync();
            }

            // Act
            bool exists;
            await using (var actContext = _fixture.CreateDbContext())
            {
                var repository = new UserRepository(actContext);
                exists = await repository.ExistsByEmailASync(
                    existingEmail,
                    CancellationToken.None
                );
            }

            //Assert
            exists.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsByEmailAsync_WhenEmailDoesNotExist_ShouldResultFalse()
        {
            // Arrange
            Email unregisteredEmail = new Email("notfound@sigti.local");

            // Act
            bool exists;
            await using (var actContext = _fixture.CreateDbContext())
            {
                var repository = new UserRepository(actContext);
                exists = await repository.ExistsByEmailASync(
                    unregisteredEmail,
                    CancellationToken.None
                );
            }

            // Assert
            exists.Should().BeFalse();
        }
    }
}

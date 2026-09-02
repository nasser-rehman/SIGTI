using FluentAssertions;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Enums;
using SIGTI.Domain.Tests.Builders;
using SIGTI.Infrastructure.Persistence.Repositories;
using SIGTI.Infrastructure.Tests.Fixtures;

namespace SIGTI.Infrastructure.Tests.Repositories
{
    [Collection("DatabaseCollection")]
    public class SupportQueueRepositoryTests : IAsyncLifetime
    {
        private readonly PostgreSqlDatabaseFixture _fixture;

        public SupportQueueRepositoryTests(PostgreSqlDatabaseFixture fixture)
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
        public async Task GetByIdAsync_WhenQueueExists_ShouldReturnQueueWithMemberLoaded()
        {
            // Arrange: Create Department, Technicians and queue with members
            var department = new DepartmentBuilder().Build();
            var techOne = new UserBuilder()
                .WithDepartment(department)
                .WithRole(Role.Technician)
                .WithEmail("tech.one@sigti.local")
                .Build();
            var techTwo = new UserBuilder()
                .WithDepartment(department)
                .WithRole(Role.Technician)
                .WithEmail("tech.two@sigti.local")
                .Build();

            var queue = new SupportQueueBuilder()
                .WithName("Tier 1 Supp")
                .Build();

            queue.AddMember(techOne, maxConcurrentTickets: 5);
            queue.AddMember(techTwo, maxConcurrentTickets: 3);

            await setupAndPersistAsync(department, techOne, techTwo, queue);

            // Act: Query queue by Id
            SupportQueue? result;
            await using (var actContext = _fixture.CreateDbContext())
            {
                var repository = new SupportQueueRepository(actContext);
                result = await repository.GetByIdAsync(
                    queue.Id,
                    CancellationToken.None
                );
            }

            result.Should().NotBeNull();
            result!.Id.Should().Be(queue.Id);
            result.Name.Should().Be("Tier 1 Supp");
            result.Members.Should().HaveCount(2);
            result
                .Members.Select(member => member.TechnicianId)
                .Should()
                .Contain(new[] { techOne.Id, techTwo.Id });
        }

        [Fact]
        public async Task GetByIdAsync_WhenQueueDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            var nonExistentQueueId = Guid.NewGuid();

            // Act
            SupportQueue? result;
            await using (var actContext = _fixture.CreateDbContext())
            {
                var repository = new SupportQueueRepository(actContext);
                result = await repository.GetByIdAsync(
                    nonExistentQueueId,
                    CancellationToken.None
                );
            }

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task ExistsByNameAsync_WhenNameMatches_ShouldReturnTrue()
        {
            // Arrange
            var queueName = "Infra Support";
            var queue = new SupportQueueBuilder().WithName(queueName).Build();

            await using (var setupContext = _fixture.CreateDbContext())
            {
                await setupContext.SupportQueues.AddAsync(queue);
                await setupContext.SaveChangesAsync();
            }

            //Act
            bool exists;
            await using (var actContext = _fixture.CreateDbContext())
            {
                var repository = new SupportQueueRepository(actContext);
                exists = await repository.ExistsByNameAsync(
                    queueName,
                    CancellationToken.None
                );
            }

            exists.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsByNameAsync_WhenNameDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            var noName = "noname supp line";

            // act
            bool exists;
            await using (var actContext = _fixture.CreateDbContext())
            {
                var repository = new SupportQueueRepository(actContext);
                exists = await repository.ExistsByNameAsync(
                    noName,
                    CancellationToken.None
                );
            }

            // assert
            exists.Should().BeFalse();
        }

        private async Task setupAndPersistAsync(
            Department department,
            User techOne,
            User techTwo,
            SupportQueue queue
        )
        {
            await using var setupContext = _fixture.CreateDbContext();
            await setupContext.Departments.AddAsync(department);
            await setupContext.Users.AddRangeAsync(techOne, techTwo);
            await setupContext.SupportQueues.AddAsync(queue);
            await setupContext.SaveChangesAsync();
        }
    }
}

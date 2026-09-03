using FluentAssertions;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Tests.Builders;
using SIGTI.Infrastructure.Persistence.Repositories;
using SIGTI.Infrastructure.Tests.Fixtures;

namespace SIGTI.Infrastructure.Tests.Repositories
{
    [Collection("DatabaseCollection")]
    public class DepartmentRepositoryTests : IAsyncLifetime
    {
        private readonly PostgreSqlDatabaseFixture _fixture;

        public DepartmentRepositoryTests(PostgreSqlDatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        public Task DisposeAsync() => Task.CompletedTask;

        public async Task InitializeAsync()
        {
            // Reset Db to initial state
            await _fixture.ResetDatabaseAsync();
        }

        [Fact]
        public async Task GetByIdAsync_WhenIdExists_ShouldReturnDepartmentLoaded()
        {
            // Arrange
            var department = new DepartmentBuilder()
                .WithName("Departamento de TI")
                .Build();

            await using (var setupContext = _fixture.CreateDbContext())
            {
                await setupContext.Departments.AddAsync(department);
                await setupContext.SaveChangesAsync();
            }

            // Act
            Department? result;
            await using (var actContext = _fixture.CreateDbContext())
            {
                var repository = new DepartmentRepository(actContext);
                result = await repository.GetByIdAsync(
                    department.Id,
                    CancellationToken.None
                );
            }
            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(department.Id);
        }

        [Fact]
        public async Task GetByIdAsync_WhenDeparmentDoesNotExists_ShouldReturnNull()
        {
            // Arrange
            var nonExistenteDepartmentId = Guid.NewGuid();

            // Act
            Department? result;
            await using (var actContext = _fixture.CreateDbContext())
            {
                var repository = new DepartmentRepository(actContext);
                result = await repository.GetByIdAsync(
                    nonExistenteDepartmentId,
                    CancellationToken.None
                );
            }

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task ExistsAsync_WhenDepartmentIdExists_ShouldReturnTrue()
        {
            // Arrange
            var department = new DepartmentBuilder().Build();

            await using (var setupContext = _fixture.CreateDbContext())
            {
                await setupContext.Departments.AddAsync(
                    department,
                    CancellationToken.None
                );
                await setupContext.SaveChangesAsync();
            }

            // Act
            bool exists;
            await using (var actContext = _fixture.CreateDbContext())
            {
                var repository = new DepartmentRepository(actContext);
                exists = await repository.ExistsAsync(
                    department.Id,
                    CancellationToken.None
                );
            }

            // Assert
            exists.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsAsync_WhenDepartmentIdDoesNotExists_ShouldReturnFalse()
        {
            // Arrange
            var nonExistenteDepartmentId = Guid.NewGuid();

            // Act
            bool exists;
            await using (var actContext = _fixture.CreateDbContext())
            {
                var repository = new DepartmentRepository(actContext);
                exists = await repository.ExistsAsync(
                    nonExistenteDepartmentId,
                    CancellationToken.None
                );
            }

            // Assert
            exists.Should().BeFalse();
        }

        [Fact]
        public async Task ExistsByNameAsync_WhenDeparmentNameExists_ShouldReturnTrue()
        {
            // Arrange
            var department = new DepartmentBuilder()
                .WithName("Departamento de TI")
                .Build();

            await using (var setupContext = _fixture.CreateDbContext())
            {
                await setupContext.Departments.AddAsync(
                    department,
                    CancellationToken.None
                );
                await setupContext.SaveChangesAsync();
            }

            // Act
            bool exists;
            await using (var actContext = _fixture.CreateDbContext())
            {
                var repository = new DepartmentRepository(actContext);
                exists = await repository.ExistsByNameAsync(
                    department.Name,
                    CancellationToken.None
                );
            }

            // Assert
            exists.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsByNameAsync_WhenDeparmentNameDoesNotExists_ShouldReturnFalse()
        {
            // Arrange
            var nonExistenteDepartmentName = "Hello World!";

            // Act
            bool exists;
            await using (var actContext = _fixture.CreateDbContext())
            {
                var repository = new DepartmentRepository(actContext);
                exists = await repository.ExistsByNameAsync(
                    nonExistenteDepartmentName,
                    CancellationToken.None
                );
            }

            // Assert
            exists.Should().BeFalse();
        }

        [Fact]
        public async Task ListAllAsync_WithoutFilters_ShouldReturnAllDepartments()
        {
            // Arrange
            var departmentOne = new DepartmentBuilder().Build();
            var departmentTwo = new DepartmentBuilder().Build();
            var departmentThree = new DepartmentBuilder().Build();
            var departmentFour = new DepartmentBuilder().Build();

            await using (var setupContext = _fixture.CreateDbContext())
            {
                await setupContext.Departments.AddRangeAsync(
                    departmentOne,
                    departmentTwo,
                    departmentThree,
                    departmentFour
                );
                await setupContext.SaveChangesAsync();
            }

            // Act
            IReadOnlyCollection<Department?> result;
            await using (var actContext = _fixture.CreateDbContext())
            {
                var repository = new DepartmentRepository(actContext);
                result = await repository.ListAllAsync(CancellationToken.None);
            }

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(4);
        }

        [Fact]
        public async Task ListAllAsync_WithoutAnyDepartment_ShouldReturnEmpty()
        {
            // Arrange: nothing to do

            // Act
            IReadOnlyCollection<Department?> result;
            await using (var actContext = _fixture.CreateDbContext())
            {
                var repository = new DepartmentRepository(actContext);
                result = await repository.ListAllAsync(CancellationToken.None);
            }

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task ListActiveAsync_WhenDepartmentIsActive_ShouldReturnActiveDepartments()
        {
            // Arrange
            var departmentOne = new DepartmentBuilder().Build();
            var departmentTwo = new DepartmentBuilder().AsDeactivated().Build();
            var departmentThree = new DepartmentBuilder()
                .AsDeactivated()
                .Build();
            var departmentFour = new DepartmentBuilder().Build();

            await using (var setupContext = _fixture.CreateDbContext())
            {
                await setupContext.Departments.AddRangeAsync(
                    departmentOne,
                    departmentTwo,
                    departmentThree,
                    departmentFour
                );
                await setupContext.SaveChangesAsync();
            }

            // Act
            IReadOnlyCollection<Department?> result;
            await using (var actContext = _fixture.CreateDbContext())
            {
                var repository = new DepartmentRepository(actContext);
                result = await repository.ListActiveAsync(
                    CancellationToken.None
                );
            }

            // Assert
            result.Should().HaveCount(2);
            result
                .Select(department => department!.Id)
                .Should()
                .Contain(new[] { departmentOne.Id, departmentFour.Id });
        }
    }
}

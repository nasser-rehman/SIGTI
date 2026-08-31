using FluentAssertions;
using SIGTI.Application.Common.Enums;
using SIGTI.Application.Features.Tickets.Queries.ListTickets;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Tests.Builders;
using SIGTI.Infrastructure.Persistence.Repositories;
using SIGTI.Infrastructure.Tests.Fixtures;
using Xunit;

namespace SIGTI.Infrastructure.Tests.Repositories
{
    [Collection("DatabaseCollection")]
    public class TicketRepositoryTests : IAsyncLifetime
    {
        private readonly PostgreSqlDatabaseFixture _fixture;

        public TicketRepositoryTests(PostgreSqlDatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        public async Task InitializeAsync()
        {
            // Limpa todas as tabelas antes de cada teste executar
            await _fixture.ResetDatabaseAsync();
        }

        public Task DisposeAsync() => Task.CompletedTask;

        public async Task ListAsync_Without_Filters_Should_Return_All_Paginated_Tickets()
        {
            // Arrange
            var department = new DepartmentBuilder().Build();
            var queue = new SupportQueueBuilder().Build();
            var user = new UserBuilder().WithDepartment(department).Build();

            var ticket1 = new TicketBuilder()
                .WithDepartment(department)
                .WithQueue(queue)
                .WithCreatedBy(user)
                .Build();
            var ticket2 = new TicketBuilder()
                .WithDepartment(department)
                .WithQueue(queue)
                .WithCreatedBy(user)
                .Build();
            var ticket3 = new TicketBuilder()
                .WithDepartment(department)
                .WithQueue(queue)
                .WithCreatedBy(user)
                .Build();

            await using (var setupContext = _fixture.CreateDbContext())
            {
                await setupContext.Departments.AddAsync(department);
                await setupContext.SupportQueues.AddAsync(queue);
                await setupContext.Users.AddAsync(user);
                await setupContext.Tickets.AddRangeAsync(ticket1, ticket2, ticket3);
                await setupContext.SaveChangesAsync();
            }

            // Act
            IReadOnlyCollection<Ticket> result;
            var filter = new TicketListFilter();

            await using (var actContext = _fixture.CreateDbContext())
            {
                var repository = new TicketRepository(actContext);
                result = await repository.ListAsync(
                    filter,
                    TicketSortField.CreatedAt,
                    SortDirection.Descending,
                    skip: 0,
                    take: 10,
                    CancellationToken.None
                );
            }

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().Contain(t => t.Id == ticket1.Id);
            result.Should().Contain(t => t.Id == ticket2.Id);
            result.Should().Contain(t => t.Id == ticket3.Id);
        }
    }
}

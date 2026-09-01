using FluentAssertions;
using SIGTI.Application.Common.Enums;
using SIGTI.Application.Features.Tickets.Queries.ListTickets;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Enums;
using SIGTI.Domain.Tests.Builders;
using SIGTI.Infrastructure.Persistence.Repositories;
using SIGTI.Infrastructure.Tests.Extensions;
using SIGTI.Infrastructure.Tests.Fixtures;

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
            // Reset database state before each test run
            await _fixture.ResetDatabaseAsync();
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task ListAsync_Without_Filters_Should_Return_All_Paginated_Tickets()
        {
            // Arrange: Seed base entities and create multiple tickets
            Ticket ticket1,
                ticket2,
                ticket3;

            await using (var setupContext = _fixture.CreateDbContext())
            {
                var seed = await setupContext.SeedBasicTicketContextAsync();

                ticket1 = new TicketBuilder()
                    .WithDepartment(seed.Department)
                    .WithQueue(seed.Queue)
                    .WithCreatedBy(seed.CreatedBy)
                    .Build();
                ticket2 = new TicketBuilder()
                    .WithDepartment(seed.Department)
                    .WithQueue(seed.Queue)
                    .WithCreatedBy(seed.CreatedBy)
                    .Build();
                ticket3 = new TicketBuilder()
                    .WithDepartment(seed.Department)
                    .WithQueue(seed.Queue)
                    .WithCreatedBy(seed.CreatedBy)
                    .Build();

                await setupContext.Tickets.AddRangeAsync(
                    ticket1,
                    ticket2,
                    ticket3
                );
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
                    cancellationToken: CancellationToken.None
                );
            }

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().Contain(t => t.Id == ticket1.Id);
            result.Should().Contain(t => t.Id == ticket2.Id);
            result.Should().Contain(t => t.Id == ticket3.Id);
        }

        [Fact]
        public async Task ListAsync_With_Technician_Filter_Should_Return_Only_Tickets_Currently_Assigned_To_Technician()
        {
            // Arrange: Seed base context and add second technician for reassignment verification
            BasicTicketContext seed;
            User techB;
            Ticket ticket1,
                ticket2,
                ticket3;

            await using (var setupContext = _fixture.CreateDbContext())
            {
                seed = await setupContext.SeedBasicTicketContextAsync();

                techB = new UserBuilder()
                    .WithDepartment(seed.Department)
                    .WithEmail("technicianB@sigti.local")
                    .Build();

                await setupContext.Users.AddAsync(techB);

                // Ticket 1: Active with seed.Technician (Tech A)
                ticket1 = new TicketBuilder()
                    .WithDepartment(seed.Department)
                    .WithQueue(seed.Queue)
                    .WithCreatedBy(seed.CreatedBy)
                    .Build();
                ticket1.AssignTechnician(
                    seed.Technician,
                    seed.CreatedBy,
                    "Initial assignment to Tech A"
                );

                // Ticket 2: Active with Tech B
                ticket2 = new TicketBuilder()
                    .WithDepartment(seed.Department)
                    .WithQueue(seed.Queue)
                    .WithCreatedBy(seed.CreatedBy)
                    .Build();
                ticket2.AssignTechnician(
                    techB,
                    seed.CreatedBy,
                    "Initial assignment to Tech B"
                );

                // Ticket 3: Assigned to Tech A first, then reassigned to Tech B
                ticket3 = new TicketBuilder()
                    .WithDepartment(seed.Department)
                    .WithQueue(seed.Queue)
                    .WithCreatedBy(seed.CreatedBy)
                    .Build();
                ticket3.AssignTechnician(
                    seed.Technician,
                    seed.CreatedBy,
                    "Initial assignment to Tech A"
                );
                ticket3.AssignTechnician(
                    techB,
                    seed.CreatedBy,
                    "Transferred to Tech B"
                );

                await setupContext.Tickets.AddRangeAsync(
                    ticket1,
                    ticket2,
                    ticket3
                );
                await setupContext.SaveChangesAsync();
            }

            // Act: Query filtering strictly by seed.Technician
            var filter = new TicketListFilter
            {
                TechnicianId = seed.Technician.Id,
            };

            IReadOnlyCollection<Ticket> result;
            await using (var actContext = _fixture.CreateDbContext())
            {
                var repository = new TicketRepository(actContext);
                result = await repository.ListAsync(
                    filter,
                    TicketSortField.CreatedAt,
                    SortDirection.Descending,
                    skip: 0,
                    take: 10,
                    cancellationToken: CancellationToken.None
                );
            }

            // Assert: Must return only ticket1
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Id.Should().Be(ticket1.Id);
        }

        [Fact]
        public async Task CountAsync_WithStatusFilter_ShouldReturnOnlyCountOfMatchingTickets()
        {
            // Arrange: Seed base context and populate tickets in multiple lifecycle states
            int totalCountNew,
                totalCountInProgress,
                totalCountResolved;

            await using (var setupContext = _fixture.CreateDbContext())
            {
                var seed = await setupContext.SeedBasicTicketContextAsync();

                // 2 tickets with status New
                var ticket1 = new TicketBuilder()
                    .WithDepartment(seed.Department)
                    .WithCreatedBy(seed.CreatedBy)
                    .WithQueue(seed.Queue)
                    .Build();
                var ticket2 = new TicketBuilder()
                    .WithDepartment(seed.Department)
                    .WithCreatedBy(seed.CreatedBy)
                    .WithQueue(seed.Queue)
                    .Build();

                // 1 ticket with status InProgress
                var ticket3 = new TicketBuilder()
                    .WithDepartment(seed.Department)
                    .WithCreatedBy(seed.CreatedBy)
                    .WithQueue(seed.Queue)
                    .Build();
                ticket3.AssignTechnician(
                    seed.Technician,
                    seed.CreatedBy,
                    "Auto assignment"
                );
                ticket3.StartService();

                // 1 ticket with status Resolved
                var ticket4 = new TicketBuilder()
                    .WithDepartment(seed.Department)
                    .WithCreatedBy(seed.CreatedBy)
                    .WithQueue(seed.Queue)
                    .Build();
                ticket4.AssignTechnician(
                    seed.Technician,
                    seed.CreatedBy,
                    "Auto assignment"
                );
                ticket4.StartService();
                ticket4.Resolve();

                await setupContext.Tickets.AddRangeAsync(
                    ticket1,
                    ticket2,
                    ticket3,
                    ticket4
                );
                await setupContext.SaveChangesAsync();
            }

            // Act
            var filterNew = new TicketListFilter { Status = TicketStatus.New };
            var filterInProgress = new TicketListFilter
            {
                Status = TicketStatus.InProgress,
            };
            var filterResolved = new TicketListFilter
            {
                Status = TicketStatus.Resolved,
            };

            await using (var actContext = _fixture.CreateDbContext())
            {
                var repository = new TicketRepository(actContext);
                totalCountNew = await repository.CountAsync(
                    filterNew,
                    cancellationToken: CancellationToken.None
                );
                totalCountInProgress = await repository.CountAsync(
                    filterInProgress,
                    cancellationToken: CancellationToken.None
                );
                totalCountResolved = await repository.CountAsync(
                    filterResolved,
                    cancellationToken: CancellationToken.None
                );
            }

            // Assert
            totalCountNew.Should().Be(2);
            totalCountInProgress.Should().Be(1);
            totalCountResolved.Should().Be(1);
        }

        [Fact]
        public async Task ListAsync_WithPrioritySortAscending_ShouldReturnTicketsOrdererByPriority()
        {
            // Arrange: Prepare tickets with distinct priorities
            Ticket ticketCritical,
                ticketHigh,
                ticketMedium,
                ticketLow;

            await using (var setupContext = _fixture.CreateDbContext())
            {
                var seed = await setupContext.SeedBasicTicketContextAsync();

                ticketCritical = new TicketBuilder()
                    .WithNumber(1)
                    .WithDepartment(seed.Department)
                    .WithQueue(seed.Queue)
                    .WithCreatedBy(seed.CreatedBy)
                    .WithPriority(TicketPriority.Critical)
                    .Build();
                ticketHigh = new TicketBuilder()
                    .WithNumber(2)
                    .WithDepartment(seed.Department)
                    .WithQueue(seed.Queue)
                    .WithCreatedBy(seed.CreatedBy)
                    .WithPriority(TicketPriority.High)
                    .Build();
                ticketMedium = new TicketBuilder()
                    .WithNumber(3)
                    .WithDepartment(seed.Department)
                    .WithQueue(seed.Queue)
                    .WithCreatedBy(seed.CreatedBy)
                    .WithPriority(TicketPriority.Medium)
                    .Build();
                ticketLow = new TicketBuilder()
                    .WithNumber(4)
                    .WithDepartment(seed.Department)
                    .WithQueue(seed.Queue)
                    .WithCreatedBy(seed.CreatedBy)
                    .WithPriority(TicketPriority.Low)
                    .Build();

                // Insert in disorder to guarantee that the database applies ORDER BY
                await setupContext.Tickets.AddRangeAsync(
                    ticketHigh,
                    ticketLow,
                    ticketCritical,
                    ticketMedium
                );
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
                    TicketSortField.Priority,
                    SortDirection.Ascending,
                    skip: 0,
                    take: 10,
                    cancellationToken: CancellationToken.None
                );
            }

            // Assert: Verify ascending priority order
            result.Should().NotBeNull();
            result.Should().HaveCount(4);
            result
                .Select(ticket => ticket.Priority)
                .Should()
                .ContainInOrder(
                    TicketPriority.Low,
                    TicketPriority.Medium,
                    TicketPriority.High,
                    TicketPriority.Critical
                );
        }

        [Fact]
        public async Task GetByIdAsync_WhenTicketExists_ShouldReturnTicketWithAllNavigationsLoaded()
        {
            // Arrange: Build aggregate graph with department, queue, assignments, comments, and audit histories
            BasicTicketContext seed;
            Ticket ticket;

            await using (var setupContext = _fixture.CreateDbContext())
            {
                seed = await setupContext.SeedBasicTicketContextAsync();

                ticket = new TicketBuilder()
                    .WithDepartment(seed.Department)
                    .WithQueue(seed.Queue)
                    .WithCreatedBy(seed.CreatedBy)
                    .Build();

                // Attach assignment history
                ticket.AssignTechnician(
                    seed.Technician,
                    seed.CreatedBy,
                    "Initial assignment for triage"
                );

                // Attach comment
                var comment = new Comment(
                    "Ticket assigned to infrastructure specialist.",
                    ticket,
                    seed.CreatedBy
                );
                ticket.AddComment(comment);

                await setupContext.Tickets.AddAsync(ticket);
                await setupContext.SaveChangesAsync();
            }

            // Act: Query ticket by ID in an isolated DbContext
            Ticket? result;
            await using (var actContext = _fixture.CreateDbContext())
            {
                var repository = new TicketRepository(actContext);
                result = await repository.GetByIdAsync(
                    ticket.Id,
                    CancellationToken.None
                );
            }

            // Assert: Verify root entity and all eager-loaded navigation properties
            result.Should().NotBeNull();
            result!.Id.Should().Be(ticket.Id);

            // Direct navigations
            result.Department.Should().NotBeNull();
            result.Department.Id.Should().Be(seed.Department.Id);

            result.CreatedBy.Should().NotBeNull();
            result.CreatedBy.Id.Should().Be(seed.CreatedBy.Id);

            result.Queue.Should().NotBeNull();
            result.Queue.Id.Should().Be(seed.Queue.Id);

            // Collection: Assignments + nested navigations (Technician & AssignedBy)
            result.Assignments.Should().HaveCount(1);
            var loadedAssignment = result.Assignments.First();
            loadedAssignment.Technician.Should().NotBeNull();
            loadedAssignment.Technician.Id.Should().Be(seed.Technician.Id);
            loadedAssignment.AssignedBy.Should().NotBeNull();
            loadedAssignment.AssignedBy.Id.Should().Be(seed.CreatedBy.Id);

            // Collection: Comments + Author navigation
            result.Comments.Should().HaveCount(1);
            var loadedComment = result.Comments.First();
            loadedComment.Author.Should().NotBeNull();
            loadedComment.Author.Id.Should().Be(seed.CreatedBy.Id);

            // Collection: Histories (Audit trail)
            result.Assignments.Should().NotBeNull();
            result.Assignments.Should().NotBeEmpty();
        }
    }
}

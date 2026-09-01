using FluentAssertions;
using SIGTI.Application.Common.Enums;
using SIGTI.Application.Features.Tickets.Queries.ListTickets;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Enums;
using SIGTI.Domain.Tests.Builders;
using SIGTI.Infrastructure.Persistence.Repositories;
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
            // Limpa todas as tabelas antes de cada teste executar
            await _fixture.ResetDatabaseAsync();
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
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

        [Fact]
        public async Task ListAsync_With_Technician_Filter_Should_Return_Only_Tickets_Currently_Assigned_To_Technician()
        {
            // Arrange
            var department = new DepartmentBuilder().Build();
            var queue = new SupportQueueBuilder().WithName("Fila N 2").Build();

            var createdBy = new UserBuilder()
                .WithDepartment(department)
                .WithEmail("createdBy@sigti.local")
                .Build();
            var techA = new UserBuilder()
                .WithDepartment(department)
                .WithEmail("technicianA@sigti.local")
                .Build();
            var techB = new UserBuilder()
                .WithDepartment(department)
                .WithEmail("technicianB@sigti.local")
                .Build();

            // Ticket 1: Active with TechA
            var ticket1 = new TicketBuilder()
                .WithDepartment(department)
                .WithQueue(queue)
                .WithCreatedBy(createdBy)
                .Build();

            ticket1.AssignTechnician(
                techA,
                createdBy,
                "Initial assignment to Tech A"
            );

            // Ticket 2: Active with TechB
            var ticket2 = new TicketBuilder()
                .WithDepartment(department)
                .WithQueue(queue)
                .WithCreatedBy(createdBy)
                .Build();

            ticket2.AssignTechnician(
                techB,
                createdBy,
                "Initial assignment to Tech B"
            );

            // Ticket 3: Assign to TechA first, then reassigned to TechB
            var ticket3 = new TicketBuilder()
                .WithDepartment(department)
                .WithQueue(queue)
                .WithCreatedBy(createdBy)
                .Build();

            ticket3.AssignTechnician(
                techA,
                createdBy,
                "Initial assignment to Tech A"
            );

            ticket3.AssignTechnician(techB, createdBy, "Transferred to Tech B");

            await using (var setupContext = _fixture.CreateDbContext())
            {
                await setupContext.Departments.AddAsync(department);
                await setupContext.SupportQueues.AddAsync(queue);
                await setupContext.Users.AddRangeAsync(createdBy, techA, techB);
                await setupContext.Tickets.AddRangeAsync(
                    ticket1,
                    ticket2,
                    ticket3
                );
                await setupContext.SaveChangesAsync();
            }

            // Act: Query filtering by TechA
            var filter = new TicketListFilter { TechnicianId = techA.Id };

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

            // Assert: Must return only Ticket 1
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Id.Should().Be(ticket1.Id);
        }

        [Fact]
        public async Task CountAsync_WithStatusFilter_ShouldReturnOnlyCountOfMatchingTickets()
        {
            // arrange
            var department = new DepartmentBuilder().Build();
            var queue = new SupportQueueBuilder().Build();
            var createdBy = new UserBuilder()
                .WithDepartment(department)
                .Build();

            var tech = new UserBuilder()
                .WithDepartment(department)
                .WithEmail("technicianTI@sigti.local")
                .Build();

            // 2 tickets with status New
            var ticket1 = new TicketBuilder()
                .WithDepartment(department)
                .WithCreatedBy(createdBy)
                .WithQueue(queue)
                .Build();
            var ticket2 = new TicketBuilder()
                .WithDepartment(department)
                .WithCreatedBy(createdBy)
                .WithQueue(queue)
                .Build();

            // 1 Ticket with status InProgress
            var ticket3 = new TicketBuilder()
                .WithDepartment(department)
                .WithCreatedBy(createdBy)
                .WithQueue(queue)
                .Build();

            ticket3.AssignTechnician(tech, createdBy, "Auto assignment");
            ticket3.StartService();

            // 1 Ticket with status Resolved
            var ticket4 = new TicketBuilder()
                .WithDepartment(department)
                .WithCreatedBy(createdBy)
                .WithQueue(queue)
                .Build();

            ticket4.AssignTechnician(tech, createdBy, "Auto assignment");
            ticket4.StartService();
            ticket4.Resolve();

            await using (var setupContext = _fixture.CreateDbContext())
            {
                await setupContext.Departments.AddAsync(department);
                await setupContext.SupportQueues.AddAsync(queue);
                await setupContext.Users.AddRangeAsync(createdBy, tech);
                await setupContext.Tickets.AddRangeAsync(
                    ticket1,
                    ticket2,
                    ticket3,
                    ticket4
                );
                await setupContext.SaveChangesAsync();
            }

            // act
            int totalCountNew,
                totalCountInProgress,
                totalCountResolved;
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
            // Arrange
            var department = new DepartmentBuilder().Build();
            var queue = new SupportQueueBuilder().Build();
            var user = new UserBuilder().WithDepartment(department).Build();

            var ticketCritical = new TicketBuilder()
                .WithNumber(1)
                .WithDepartment(department)
                .WithQueue(queue)
                .WithCreatedBy(user)
                .WithPriority(TicketPriority.Critical)
                .Build();

            var ticketHigh = new TicketBuilder()
                .WithNumber(2)
                .WithDepartment(department)
                .WithQueue(queue)
                .WithCreatedBy(user)
                .WithPriority(TicketPriority.High)
                .Build();

            var ticketMedium = new TicketBuilder()
                .WithNumber(3)
                .WithDepartment(department)
                .WithQueue(queue)
                .WithCreatedBy(user)
                .WithPriority(TicketPriority.Medium)
                .Build();

            var ticketLow = new TicketBuilder()
                .WithNumber(4)
                .WithDepartment(department)
                .WithQueue(queue)
                .WithCreatedBy(user)
                .WithPriority(TicketPriority.Low)
                .Build();

            await using (var setupContext = _fixture.CreateDbContext())
            {
                await setupContext.Departments.AddAsync(department);
                await setupContext.SupportQueues.AddAsync(queue);
                await setupContext.Users.AddAsync(user);

                // Insert in disorder to garante that the db applies the ORDER BY
                await setupContext.Tickets.AddRangeAsync(
                    ticketHigh,
                    ticketLow,
                    ticketCritical,
                    ticketMedium
                );
                await setupContext.SaveChangesAsync();

                // act
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

                // Assert: check if sequence of priorities returned is ascending
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
        }

        [Fact]
        public async Task GetByIdAsync_WhenTicketExists_ShouldReturnTicketWithAllNavigationsLoaded()
        {
            // Arrange: Build aggregate graph with department, queue, assignments, comments, and audit histories
            var department = new DepartmentBuilder().Build();
            var queue = new SupportQueueBuilder().Build();

            var createdBy = new UserBuilder()
                .WithEmail("create@sigti.local")
                .WithDepartment(department)
                .WithRole(Role.User)
                .Build();
            var assigner = new UserBuilder()
                .WithEmail("administrator@sigti.local")
                .WithRole(Role.Administrator)
                .WithDepartment(department)
                .Build();
            var technician = new UserBuilder()
                .WithEmail("tech@sigti.local")
                .WithDepartment(department)
                .WithRole(Role.Technician)
                .Build();

            var ticket = new TicketBuilder()
                .WithDepartment(department)
                .WithQueue(queue)
                .WithCreatedBy(createdBy)
                .Build();

            // Attach assignment history
            ticket.AssignTechnician(
                technician,
                assigner,
                "Initial assignment for triage"
            );

            // Attach comment by the assigner (e.g., triage technician / manager)
            var comment = new Comment(
                "Ticket assigned to infrastructure specialist.",
                ticket,
                assigner
            );
            ticket.AddComment(comment);

            await using (var setupContext = _fixture.CreateDbContext())
            {
                await setupContext.Departments.AddAsync(department);
                await setupContext.SupportQueues.AddAsync(queue);
                await setupContext.Users.AddRangeAsync(
                    createdBy,
                    assigner,
                    technician
                );
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
            result.Department.Id.Should().Be(department.Id);

            result.CreatedBy.Should().NotBeNull();
            result.CreatedBy.Id.Should().Be(createdBy.Id);

            result.Queue.Should().NotBeNull();
            result.Queue.Id.Should().Be(queue.Id);

            // Collection: Assignments + nested navigations (Technician & AssignedBy)
            result.Assignments.Should().HaveCount(1);
            var loadedAssignment = result.Assignments.First();
            loadedAssignment.Technician.Should().NotBeNull();
            loadedAssignment.Technician.Id.Should().Be(technician.Id);
            loadedAssignment.AssignedBy.Should().NotBeNull();
            loadedAssignment.AssignedBy.Id.Should().Be(assigner.Id);

            // Collection: Comments + Author navigation
            result.Comments.Should().HaveCount(1);
            var loadedComment = result.Comments.First();
            loadedComment.Author.Should().NotBeNull();
            loadedComment.Author.Id.Should().Be(assigner.Id);

            // Collection: Histories (Audit trail)
            result.Assignments.Should().NotBeNull();
            result.Assignments.Should().NotBeEmpty();
        }
    }
}

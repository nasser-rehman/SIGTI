using FluentAssertions;
using Moq;
using SIGTI.Application.Common.Exceptions;
using SIGTI.Application.Common.Interfaces.Services;
using SIGTI.Application.Features.Tickets.Queries.GetTicketTimeline;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Enums;
using SIGTI.Domain.Tests.Builders;

namespace SIGTI.Application.Tests.Features.Tickets.Queries.GetTicketTimeline
{
    public class GetTicketTimelineQueryHandlerTests
    {
        private readonly Mock<IEntityReferenceService> _entityReferenceServiceMock;
        private readonly GetTicketTimelineQueryHandler _handler;

        public GetTicketTimelineQueryHandlerTests()
        {
            _entityReferenceServiceMock = new Mock<IEntityReferenceService>();
            _handler = new GetTicketTimelineQueryHandler(
                _entityReferenceServiceMock.Object
            );
        }

        [Fact]
        public async Task Handle_WhenTicketExistsWithFullLifecycle_ShouldReturnChronologicallyOrderedEvents()
        {
            // Arrange
            var department = new DepartmentBuilder().Build();
            var queue = new SupportQueueBuilder().Build();
            var creator = new UserBuilder()
                .WithDepartment(department)
                .WithEmail("user@sigti.local")
                .WithRole(Role.User)
                .WithName("Requesting User")
                .Build();
            var technician = new UserBuilder()
                .WithDepartment(department)
                .WithEmail("tech@sigti.local")
                .WithRole(Role.Technician)
                .WithName("Technician in charge")
                .Build();
            var admin = new UserBuilder()
                .WithDepartment(department)
                .WithEmail("admin@sigti.local")
                .WithRole(Role.Administrator)
                .WithName("Administrator")
                .Build();

            var ticket = new TicketBuilder()
                .WithDepartment(department)
                .WithQueue(queue)
                .WithCreatedBy(creator)
                .WithTitle("Internet Connection Problem")
                .Build();

            // Simulate the complete lifecycle via domain invariants
            ticket.AssignTechnician(technician, admin, "Initial order");
            ticket.StartService();
            ticket.AddComment(
                new Comment("Analyzing local switches.", ticket, technician)
            );
            ticket.AddComment(
                new Comment("Okay i'm waiting.", ticket, creator)
            );
            ticket.AddComment(
                new Comment(
                    "Done. Please restart your computer and try navigate again.",
                    ticket,
                    technician
                )
            );
            ticket.Resolve();
            ticket.Close();

            _entityReferenceServiceMock
                .Setup(s =>
                    s.GetRequiredTicketAsync(
                        ticket.Id,
                        It.IsAny<CancellationToken>()
                    )
                )
                .ReturnsAsync(ticket);

            var query = new GetTicketTimelineQuery(ticket.Id);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.TicketId.Should().Be(ticket.Id);
            result.TicketCode.Should().Be(ticket.Code);
            result.Title.Should().Be("Internet Connection Problem");
            result.Status.Should().Be(TicketStatus.Closed);

            // 8 events: created, assigned, started, 3 comments, resolved, closed
            result.Timeline.Should().HaveCount(8);
            result.Timeline.Should().BeInAscendingOrder(e => e.Timestamp);

            var eventTypes = result.Timeline.Select(e => e.EventType).ToList();
            eventTypes
                .Should()
                .ContainInOrder(
                    "Created",
                    "Assigned",
                    "Started",
                    "CommentAdded",
                    "CommentAdded",
                    "CommentAdded",
                    "Resolved",
                    "Closed"
                );
        }

        [Fact]
        public async Task Handle_WhenTicketOnlyCreated_ShouldReturnSingleCreatedEvent()
        {
            // Arrange
            var department = new DepartmentBuilder().Build();
            var creator = new UserBuilder()
                .WithDepartment(department)
                .WithEmail("user@sigti.local")
                .Build();
            var ticket = new TicketBuilder()
                .WithDepartment(department)
                .WithCreatedBy(creator)
                .Build();

            _entityReferenceServiceMock
                .Setup(s =>
                    s.GetRequiredTicketAsync(
                        ticket.Id,
                        It.IsAny<CancellationToken>()
                    )
                )
                .ReturnsAsync(ticket);

            var query = new GetTicketTimelineQuery(ticket.Id);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Timeline.Should().HaveCount(1);
            result.Timeline.First().EventType.Should().Be("Created");
            result.Timeline.First().ActorName.Should().Be(creator.Name);
        }

        [Fact]
        public async Task Handle_WhenTicketNotFound_ShouldThrowNotFoundException()
        {
            // Arrange
            var nonExistentTicketId = Guid.NewGuid();

            _entityReferenceServiceMock
                .Setup(s =>
                    s.GetRequiredTicketAsync(
                        nonExistentTicketId,
                        It.IsAny<CancellationToken>()
                    )
                )
                .ThrowsAsync(
                    new NotFoundException(nameof(Ticket), nonExistentTicketId)
                );

            var query = new GetTicketTimelineQuery(nonExistentTicketId);

            // Act
            var act = () => _handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}

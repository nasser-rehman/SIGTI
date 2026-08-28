using FluentAssertions;
using Moq;
using SIGTI.Application.Common.Exceptions;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Application.Features.Tickets.Queries.GetTicketById;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Tests.Builders;

namespace SIGTI.Application.Tests.Features.Tickets.Queries.GetTicketById
{
    public sealed class GetTicketByIdHandlerTests
    {
        [Fact]
        public async Task Should_Return_Ticket_When_Found()
        {
            // Arrange
            var repository = new Mock<ITicketRepository>();

            var ticket = new TicketBuilder()
                .WithNumber(10)
                .Build();

            repository
                .Setup(x => x.GetDetailsByIdAsync(
                    ticket.Id,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(ticket);

            var handler = new GetTicketByIdHandler(repository.Object);

            var query = new GetTicketByIdQuery(ticket.Id);

            //Act
            var result = await handler.Handle(
                query,
                CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(ticket.Id);
            result.Number.Should().Be(ticket.Number);
            result.Title.Should().Be(ticket.Title);
            result.Description.Should().Be(ticket.Description);
            result.Status.Should().Be(ticket.Status.ToString());
            result.Priority.Should().Be(ticket.Priority.ToString());
            result.Category.Should().Be(ticket.Category.ToString());
            result.Department.Should().Be(ticket.Department.Name);
            result.Queue.Should().Be(ticket.Queue.Name);
            result.CreatedBy.Should().Be(ticket.CreatedBy.Name);
            result.CreatedAt.Should().Be(ticket.CreatedAt);

            repository.Verify(
                x => x.GetDetailsByIdAsync(
                    ticket.Id,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Should_Throw_When_Ticket_Is_Not_Found()
        {
            // Arrange
            var repository = new Mock<ITicketRepository>();

            var ticketId = Guid.NewGuid();

            repository
                .Setup(x => x.GetDetailsByIdAsync(
                    ticketId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Ticket?)null);

            var handler = new GetTicketByIdHandler(
                repository.Object);

            var query = new GetTicketByIdQuery(ticketId);

            // Act
            var action = () => handler.Handle(
                query,
                CancellationToken.None);

            // Assert
            await action.Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage("Chamado não encontrado.");

            repository.Verify(
                x => x.GetDetailsByIdAsync(
                    ticketId,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}

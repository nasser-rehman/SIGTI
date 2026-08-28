using FluentAssertions;
using Moq;
using SIGTI.Application.Common.Exceptions;
using SIGTI.Application.Tests.Fixtures;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Enums;

namespace SIGTI.Application.Tests.Features.Tickets.Commands.CreateTicket
{
    public sealed class CreateTicketHandlerTests
    {
        [Fact]
        public async Task Should_Create_Ticket_Successfully()
        {
            // Arrange
            var fixture = new CreateTicketHandlerFixture();


            // Act
            var result = await fixture.Handler.Handle(
                fixture.Command,
                CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Title.Should().Be(fixture.Command.Title);
            result.Status.Should().Be(TicketStatus.Assigned);

            fixture.TicketRepository.Verify(
                x => x.AddAsync(
                    It.Is<Ticket>(ticket =>
                    ticket.Number == fixture.TicketNumber &&
                    ticket.Title == fixture.Command.Title &&
                    ticket.Description == fixture.Command.Description &&
                    ticket.Priority == fixture.Command.Priority &&
                    ticket.Category == fixture.Command.Category &&
                    ticket.DepartmentId == fixture.Command.DepartmentId &&
                    ticket.QueueId == fixture.Command.QueueId &&
                    ticket.CreatedById == fixture.Command.CreatedById),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            fixture.UnitOfWork.Verify(
                x => x.SaveChangesAsync(
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Should_Not_Save_When_Department_Is_Not_Found()
        {
            // Arrange
            var fixture = new CreateTicketHandlerFixture();


            fixture.EntityReferenceService
                .Setup(x => x.GetRequiredDepartmentAsync(
                    fixture.Command.DepartmentId,
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(
                    new NotFoundException("Departamento não encontrado."));

            // Act
            var action = () => fixture.Handler.Handle(
                fixture.Command,
                CancellationToken.None);

            // Assert
            await action.Should()
                .ThrowAsync<NotFoundException>();

            fixture.TicketRepository.Verify(
                x => x.AddAsync(
                    It.IsAny<Ticket>(),
                    It.IsAny<CancellationToken>()), Times.Never);

            fixture.UnitOfWork.Verify(
                x => x.SaveChangesAsync(
                    It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}

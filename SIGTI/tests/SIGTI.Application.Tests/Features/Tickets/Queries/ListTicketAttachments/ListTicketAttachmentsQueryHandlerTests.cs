using FluentAssertions;
using Moq;
using SIGTI.Application.Common.Interfaces.Services;
using SIGTI.Application.Features.Tickets.Queries.ListTicketAttachments;
using SIGTI.Domain.Tests.Builders;
using Xunit;

namespace SIGTI.Application.Tests.Features.Tickets.Queries.ListTicketAttachments;

public class ListTicketAttachmentsQueryHandlerTests
{
    private readonly Mock<IEntityReferenceService> _entityReferenceServiceMock;
    private readonly ListTicketAttachmentsQueryHandler _handler;

    public ListTicketAttachmentsQueryHandlerTests()
    {
        _entityReferenceServiceMock = new Mock<IEntityReferenceService>();
        _handler = new ListTicketAttachmentsQueryHandler(
            _entityReferenceServiceMock.Object
        );
    }

    [Fact]
    public async Task Handle_WhenTicketHasAttachments_ShouldReturnChronologicallyOrderedList()
    {
        // Arrange
        var user = new UserBuilder().Build();
        var ticket = new TicketBuilder().WithCreatedBy(user).Build();

        var attachment1 = new AttachmentBuilder()
            .WithFileName("primeiro.png")
            .WithTicket(ticket)
            .WithUploadedBy(user)
            .Build();

        var attachment2 = new AttachmentBuilder()
            .WithFileName("segundo.pdf")
            .WithTicket(ticket)
            .WithUploadedBy(user)
            .Build();

        ticket.AddAttachment(attachment1);
        ticket.AddAttachment(attachment2);

        _entityReferenceServiceMock
            .Setup(x =>
                x.GetRequiredTicketAsync(
                    ticket.Id,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(ticket);

        var query = new ListTicketAttachmentsQuery(ticket.Id);

        // Act
        var response = await _handler.Handle(query, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Should().HaveCount(2);
        response[0].FileName.Should().Be("primeiro.png");
        response[1].FileName.Should().Be("segundo.pdf");
    }

    [Fact]
    public async Task Handle_WhenTicketHasNoAttachments_ShouldReturnEmptyList()
    {
        // Arrange
        var user = new UserBuilder().Build();
        var ticket = new TicketBuilder().WithCreatedBy(user).Build();

        _entityReferenceServiceMock
            .Setup(x =>
                x.GetRequiredTicketAsync(
                    ticket.Id,
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(ticket);

        var query = new ListTicketAttachmentsQuery(ticket.Id);

        // Act
        var response = await _handler.Handle(query, CancellationToken.None);

        // Assert
        response.Should().BeEmpty();
    }
}

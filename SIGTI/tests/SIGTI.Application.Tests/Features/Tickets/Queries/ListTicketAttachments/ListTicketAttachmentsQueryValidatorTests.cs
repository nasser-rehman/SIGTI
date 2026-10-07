using FluentAssertions;
using SIGTI.Application.Features.Tickets.Queries.ListTicketAttachments;
using Xunit;

namespace SIGTI.Application.Tests.Features.Tickets.Queries.ListTicketAttachments;

public class ListTicketAttachmentsQueryValidatorTests
{
    private readonly ListTicketAttachmentsQueryValidator _validator = new();

    [Fact]
    public void Validate_WhenValidQuery_ShouldPass()
    {
        var query = new ListTicketAttachmentsQuery(Guid.NewGuid());
        var result = _validator.Validate(query);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WhenTicketIdEmpty_ShouldFail()
    {
        var query = new ListTicketAttachmentsQuery(Guid.Empty);
        var result = _validator.Validate(query);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "TicketId");
    }
}

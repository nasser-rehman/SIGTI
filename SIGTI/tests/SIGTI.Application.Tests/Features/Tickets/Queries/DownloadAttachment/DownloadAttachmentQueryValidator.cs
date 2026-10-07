using FluentAssertions;
using SIGTI.Application.Features.Tickets.Queries.DownloadAttachment;
using Xunit;

namespace SIGTI.Application.Tests.Features.Tickets.Queries.DownloadAttachment;

public class DownloadAttachmentQueryValidatorTests
{
    private readonly DownloadAttachmentQueryValidator _validator = new();

    [Fact]
    public void Validate_WhenValidQuery_ShouldPass()
    {
        var query = new DownloadAttachmentQuery(Guid.NewGuid(), Guid.NewGuid());
        var result = _validator.Validate(query);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WhenTicketIdEmpty_ShouldFail()
    {
        var query = new DownloadAttachmentQuery(Guid.Empty, Guid.NewGuid());
        var result = _validator.Validate(query);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "TicketId");
    }

    [Fact]
    public void Validate_WhenAttachmentIdEmpty_ShouldFail()
    {
        var query = new DownloadAttachmentQuery(Guid.NewGuid(), Guid.Empty);
        var result = _validator.Validate(query);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "AttachmentId");
    }
}

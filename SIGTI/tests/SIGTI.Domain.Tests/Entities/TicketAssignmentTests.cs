using System;
using FluentAssertions;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Exceptions;
using SIGTI.Domain.Tests.Builders;
using Xunit;

namespace SIGTI.Domain.Tests.Entities
{
    public class TicketAssignmentTests
    {
        [Fact]
        public void Should_Create_A_New_TicketAssignment()
        {
            var assignment = new TicketAssignmentBuilder().Build();

            assignment.TicketId.Should().NotBeEmpty();
            assignment.TechnicianId.Should().NotBeEmpty();
            assignment.AssignedById.Should().NotBeEmpty();
            assignment.Reason.Should().Be("Atribuição inicial ao N1");
            assignment.AssignedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
            assignment.FinishedAt.Should().BeNull();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Should_Not_Update_TicketAssignment_With_Null_Or_Whitespace_Reason(
            string invalidReason
        )
        {
            var assignment = new TicketAssignmentBuilder().Build();
            Action action = () => assignment.UpdateReason(invalidReason);
            action
                .Should()
                .Throw<DomainException>()
                .WithMessage("A razão da atribuição não pode ser vazia.");
        }

        [Fact]
        public void Should_Mark_As_Finished()
        {
            var assignment = new TicketAssignmentBuilder().Build();

            Action action = () => assignment.MarkAsFinished();

            action.Should().NotThrow<DomainException>();
            assignment.FinishedAt.Should().NotBeNull();
            assignment
                .FinishedAt.Value.Should()
                .BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        }
    }
}

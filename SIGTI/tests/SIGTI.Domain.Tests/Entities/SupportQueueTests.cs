using FluentAssertions;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Exceptions;
using SIGTI.Domain.Tests.Builders;
using Xunit;
using System;

namespace SIGTI.Domain.Tests.Entities
{
    public class SupportQueueTests
    {
        [Fact]
        public void Should_Create_A_New_SupportQueue()
        {
            var queue = new SupportQueueBuilder().Build();

            queue.Name.Should().Be("Fila N1");
            queue.Description.Should().Be("Atendimento de primeiro nível (helpdesk).");
            queue.IsActive.Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Should_Not_Create_SupportQueue_With_Null_Or_Whitespace_Name(string invalidName)
        {
            Action action = () => new SupportQueueBuilder().WithName(invalidName).Build();
            action.Should().Throw<DomainException>().WithMessage("O nome da fila de suporte é obrigatório.");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Should_Not_Create_SupportQueue_With_Null_Or_Whitespace_Description(string invalidDescription)
        {
            Action action = () => new SupportQueueBuilder().WithDescription(invalidDescription).Build();
            action.Should().Throw<DomainException>().WithMessage("A descrição da fila de suporte é obrigatória.");
        }
    }
}

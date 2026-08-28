using FluentAssertions;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Exceptions;
using SIGTI.Domain.Tests.Builders;
using Xunit;

namespace SIGTI.Domain.Tests.Entities
{
    public class CommentTests
    {
        [Fact]
        public void Should_Create_A_New_Comment()
        {
            var comment = new CommentBuilder().Build();

            comment.Content.Should().Be("Conteúdo padrão do comentário para os testes.");
            comment.Ticket.Should().NotBeNull();
            comment.Author.Should().NotBeNull();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Should_Not_Create_Comment_With_Null_Or_Whitespace_Content(string invalidContent)
        {
            Action action = () => new CommentBuilder().WithContent(invalidContent).Build();
            action
                .Should()
                .Throw<DomainException>()
                .WithMessage("O conteúdo do comentário é obrigatório.");
        }

        [Fact]
        public void Should_Not_Create_Comment_With_Null_Ticket()
        {
            Action action = () => new Comment("Content", null!, new UserBuilder().Build());
            action.Should().Throw<DomainException>();
        }

        [Fact]
        public void Should_Not_Create_Comment_With_Null_Author()
        {
            Action action = () => new Comment("Content", new TicketBuilder().Build(), null!);
            action.Should().Throw<DomainException>();
        }
    }
}

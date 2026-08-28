using System;
using SIGTI.Domain.Entities;

namespace SIGTI.Domain.Tests.Builders
{
    public class CommentBuilder
    {
        private string _content = "Conteúdo padrão do comentário para os testes.";
        private Ticket? _ticket;
        private User? _author;


        public CommentBuilder WithContent(string content)
        {
            _content = content;
            return this;
        }

        public CommentBuilder WithTicket(Ticket ticket)
        {
            _ticket = ticket;
            return this;
        }

        public CommentBuilder WithAuthor(User author)
        {
            _author = author;
            return this;
        }

        public Comment Build()
        {
            var ticket = _ticket ?? new TicketBuilder().Build();
            var author = _author ?? new UserBuilder().Build();
            return new Comment(_content, ticket, author);
        }
    }
}

using System;
using FluentAssertions;
using SIGTI.Domain.Exceptions;
using SIGTI.Domain.ValueObjects;
using Xunit;

namespace SIGTI.Domain.Tests.ValueObjects
{
    public class EmailTests
    {
        [Theory]
        [InlineData("teste@exemplo.com")]
        [InlineData("usuario.nome@empresa.com.br")]
        [InlineData("email_valido123@dominio.org")]
        public void Should_Create_Email_With_Valid_Format(string validEmail)
        {
            var email = new Email(validEmail);

            email.Value.Should().Be(validEmail);
        }

        [Fact]
        public void Should_Create_Email_And_Trim_Whitespace()
        {
            var email = new Email("  teste@exemplo.com  ");

            email.Value.Should().Be("teste@exemplo.com");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("email-invalido")]
        [InlineData("@dominio.com")]
        [InlineData("usuario@.com")]
        public void Should_Not_Create_Email_With_Invalid_Format(string invalidEmail)
        {
            Action action = () => new Email(invalidEmail);

            action.Should().Throw<DomainException>().WithMessage("O E-mail informado é invalido.");
        }

        [Fact]
        public void Should_Not_Create_Email_With_Null_Value()
        {
            Action action = () => new Email(null!);
            action.Should().Throw<Exception>();
        }

        [Fact]
        public void Should_Not_Create_Email_Exceeding_Max_Length()
        {
            var localPart = new string('a', 200);
            var domainPart = new string('b', 60) + ".com";
            var longEmail = $"{localPart}@{domainPart}"; // Length: 200 + 1 + 60 + 4 = 265 > 254

            Action action = () => new Email(longEmail);

            action.Should().Throw<DomainException>().WithMessage("O E-mail informado é invalido.");
        }
    }
}

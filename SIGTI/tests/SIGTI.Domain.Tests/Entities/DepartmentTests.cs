using FluentAssertions;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Exceptions;
using SIGTI.Domain.Tests.Builders;
using Xunit;
using System;

namespace SIGTI.Domain.Tests.Entities
{
    public class DepartmentTests
    {
        [Fact]
        public void Should_Create_A_New_Department()
        {
            var department = new DepartmentBuilder().Build();

            department.Name.Should().Be("Departamento de TI");
            department.Description.Should().Be("Responsável por manter a infraestrutura e sistemas.");
            department.IsActive.Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Should_Not_Create_Department_With_Null_Or_Whitespace_Name(string invalidName)
        {
            Action action = () => new DepartmentBuilder().WithName(invalidName).Build();
            action.Should().Throw<DomainException>().WithMessage("O nome do departamento é obrigatório.");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Should_Not_Create_Department_With_Null_Or_Whitespace_Description(string invalidDescription)
        {
            Action action = () => new DepartmentBuilder().WithDescription(invalidDescription).Build();
            action.Should().Throw<DomainException>().WithMessage("A descrição do departamento é obrigatória.");
        }

        [Fact]
        public void Should_Deactivate_Department()
        {
            var department = new DepartmentBuilder().Build();

            Action action = () => department.Deactivate();

            action.Should().NotThrow<DomainException>();
            department.IsActive.Should().BeFalse();
        }

        [Fact]
        public void Should_Not_Deactivate_Already_Inactive_Department()
        {
            var department = new DepartmentBuilder().AsDeactivated().Build();

            Action action = () => department.Deactivate();

            action.Should().Throw<DomainException>().WithMessage("O departamento já está inativo.");
        }

        [Fact]
        public void Should_Activate_Department()
        {
            var department = new DepartmentBuilder().AsDeactivated().Build();

            Action action = () => department.Activate();

            action.Should().NotThrow<DomainException>();
            department.IsActive.Should().BeTrue();
        }

        [Fact]
        public void Should_Not_Activate_Already_Active_Department()
        {
            var department = new DepartmentBuilder().Build();

            Action action = () => department.Activate();

            action.Should().Throw<DomainException>().WithMessage("O departamento já está ativo.");
        }
    }
}

using FluentAssertions;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Enums;
using SIGTI.Domain.Exceptions;
using SIGTI.Domain.Tests.Builders;
using Xunit;

namespace SIGTI.Domain.Tests.Entities
{
    public class UserTests
    {
        [Fact]
        public void Should_Create_A_New_User()
        {
            var user = new UserBuilder().Build();

            user.Name.Should().Be("Usuário Teste");
            user.Email.Value.Should().Be("example@example.com");
            user.PasswordHash.Should().NotBeEmpty();
            user.Role.Should().Be(Role.Technician);
            user.DepartmentId.Should().NotBeEmpty();
        }

        [Fact]
        public void Should_Update_User_Name()
        {
            var user = new UserBuilder().Build();

            Action action = () => user.UpdateName("Novo nome de Teste");

            action.Should().NotThrow<DomainException>();
            user.Name.Should().Be("Novo nome de Teste");
        }

        [Fact]
        public void Should_Not_Update_User_Name_With_Longer_Name()
        {
            var user = new UserBuilder().Build();

            Action action = () => user.UpdateName(new string('a', 101));

            action.Should().Throw<DomainException>("O nome deve ter entre 10 e 100 caracteres.");
        }

        [Fact]
        public void Should_Not_Update_User_Name_With_Shorter_Name()
        {
            var user = new UserBuilder().Build();

            Action action = () => user.UpdateName(new string('a', 9));

            action.Should().Throw<DomainException>("O nome deve ter entre 10 e 100 caracteres.");
        }

        [Fact]
        public void Should_Not_Update_User_Name_With_Null_Name()
        {
            var user = new UserBuilder().Build();

            Action action = () => user.UpdateName(null);

            action.Should().Throw<DomainException>("O nome é obrigatório.");
        }

        [Fact]
        public void Should_Update_User_Password()
        {
            var user = new UserBuilder().Build();

            Action action = () => user.UpdatePasswordHash("newHashPassword");

            action.Should().NotThrow<DomainException>();
            user.PasswordHash.Should().Be("newHashPassword");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Should_Not_Update_User_Password_With_Null_Or_Whitespace(string invalidPassword)
        {
            var user = new UserBuilder().Build();

            Action action = () => user.UpdatePasswordHash(invalidPassword);

            action.Should().Throw<DomainException>().WithMessage("A senha é obrigatória.");
        }

        [Fact]
        public void Should_Update_User_Role()
        {
            var user = new UserBuilder().Build();

            Action action = () => user.UpdateRole(Role.Administrator);

            action.Should().NotThrow<DomainException>();
            user.Role.Should().Be(Role.Administrator);
        }

        [Fact]
        public void Should_Not_Update_User_Role_With_Invalid_Role()
        {
            var user = new UserBuilder().Build();

            Action action = () => user.UpdateRole((Role)999);

            action.Should().Throw<DomainException>().WithMessage("Função inválida.");
        }

        [Fact]
        public void Should_Update_User_Department()
        {
            var user = new UserBuilder().Build();
            var department = new DepartmentBuilder().Build();

            Action action = () => user.ChangeDepartment(department);

            action.Should().NotThrow<DomainException>();
            user.DepartmentId.Should().Be(department.Id);
        }

        [Fact]
        public void Should_Not_Update_User_Department_With_Empty_Guid()
        {
            var user = new UserBuilder().Build();

            Action action = () => user.ChangeDepartment(null!);

            action.Should().Throw<DomainException>().WithMessage("O departamento é obrigatório.");
        }

        [Fact]
        public void Should_Deactivate_User()
        {
            var user = new UserBuilder().Build();

            Action action = () => user.Deactivate();

            action.Should().NotThrow<DomainException>();
            user.IsActive.Should().BeFalse();
        }

        [Fact]
        public void Should_Keep_User_Deactivated_When_Calling_Deactivate_Again()
        {
            var user = new UserBuilder().Build();
            user.Deactivate();

            Action action = () => user.Deactivate();

            action.Should().Throw<DomainException>();
            user.IsActive.Should().BeFalse();
        }

        [Fact]
        public void Should_Activate_User()
        {
            var user = new UserBuilder().Build();
            user.Deactivate();

            Action action = () => user.Activate();

            action.Should().NotThrow<DomainException>();
            user.IsActive.Should().BeTrue();
        }

        [Fact]
        public void Should_Keep_User_Activated_When_Calling_Activate_Again()
        {
            var user = new UserBuilder().Build();

            Action action = () => user.Activate();

            action.Should().Throw<DomainException>();
            user.IsActive.Should().BeTrue();
        }
    }
}

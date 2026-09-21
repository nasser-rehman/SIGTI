using System.Linq;
using FluentAssertions;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Exceptions;
using SIGTI.Domain.Tests.Builders;
using Xunit;

namespace SIGTI.Domain.Tests.Entities
{
    public class SupportQueueTests
    {
        [Fact]
        public void Should_Create_A_New_SupportQueue()
        {
            var queue = new SupportQueueBuilder().Build();

            queue.Name.Should().Be("Fila N1");
            queue
                .Description.Should()
                .Be("Atendimento de primeiro nível (helpdesk).");
            queue.IsActive.Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Should_Not_Create_SupportQueue_With_Null_Or_Whitespace_Name(
            string invalidName
        )
        {
            Action action = () =>
                new SupportQueueBuilder().WithName(invalidName).Build();
            action
                .Should()
                .Throw<DomainException>()
                .WithMessage("O nome da fila de suporte é obrigatório.");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Should_Not_Create_SupportQueue_With_Null_Or_Whitespace_Description(
            string invalidDescription
        )
        {
            Action action = () =>
                new SupportQueueBuilder()
                    .WithDescription(invalidDescription)
                    .Build();
            action
                .Should()
                .Throw<DomainException>()
                .WithMessage("A descrição da fila de suporte é obrigatória.");
        }

        [Fact]
        public void Should_Update_Name_Successfully()
        {
            var queue = new SupportQueueBuilder().Build();
            var newName = "File N2 Especializada";

            queue.UpdateName(newName);

            queue.Name.Should().Be(newName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Should_Not_Update_Name_With_Invalid_Name(string invalidName)
        {
            var queue = new SupportQueueBuilder().Build();

            Action action = () => queue.UpdateName(invalidName);

            action
                .Should()
                .Throw<DomainException>()
                .WithMessage("O nome da fila de suporte é obrigatório.");
        }

        [Fact]
        public void Should_Not_Update_Name_Exceeding_Max_Length()
        {
            var queue = new SupportQueueBuilder().Build();
            var longName = new string('A', 151);

            Action action = () => queue.UpdateName(longName);

            action
                .Should()
                .Throw<DomainException>()
                .WithMessage(
                    "O nome da fila de suporte deve ter no máximo 150 caracteres."
                );
        }

        [Fact]
        public void Shoult_Update_Description_Succesfully()
        {
            var queue = new SupportQueueBuilder().Build();
            var newDescription = "Nova descrição da fila de atendimento.";

            queue.UpdateDescription(newDescription);

            queue.Description.Should().Be(newDescription);
            queue.UpdatedAt.Should().NotBeNull();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Should_Not_Update_Description_With_Invalid_Description(
            string invalidDescription
        )
        {
            var queue = new SupportQueueBuilder().Build();

            Action action = () => queue.UpdateDescription(invalidDescription);

            action
                .Should()
                .Throw<DomainException>()
                .WithMessage("A descrição da fila de suporte é obrigatória.");
        }

        [Fact]
        public void Should_Not_Update_Description_Exceeding_Max_Length()
        {
            var queue = new SupportQueueBuilder().Build();
            var longDescription = new string('A', 501);

            Action action = () => queue.UpdateDescription(longDescription);

            action
                .Should()
                .Throw<DomainException>()
                .WithMessage(
                    "A descrição da fila de suporte deve ter no máximo 500 caracteres."
                );
        }

        [Fact]
        public void Should_Deactivate_Queue_Successfully()
        {
            var queue = new SupportQueueBuilder().Build();

            queue.Deactivate();

            queue.IsActive.Should().BeFalse();
            queue.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void Should_Throw_When_Deactivating_Already_Inactive_Queue()
        {
            var queue = new SupportQueueBuilder().Build();
            queue.Deactivate();

            Action action = () => queue.Deactivate();

            action
                .Should()
                .Throw<DomainException>()
                .WithMessage("A fila já está inativa.");
        }

        [Fact]
        public void Should_Activate_Queue_Successfully()
        {
            var queue = new SupportQueueBuilder().Build();
            queue.Deactivate();

            queue.Activate();

            queue.IsActive.Should().BeTrue();
            queue.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void Should_Throw_When_Activating_Already_Active_Queue()
        {
            var queue = new SupportQueueBuilder().Build();

            Action action = () => queue.Activate();

            action
                .Should()
                .Throw<DomainException>()
                .WithMessage("A fila já está ativa.");
        }

        [Fact]
        public void Should_Add_Technician_To_Queue()
        {
            var queue = new SupportQueueBuilder().Build();
            var technician = new UserBuilder()
                .WithRole(SIGTI.Domain.Enums.Role.Technician)
                .Build();

            queue.AddMember(technician, 5);

            queue.Members.Should().HaveCount(1);
            queue.GetActiveMemberCount().Should().Be(1);
            var member = queue.Members.First();
            member.TechnicianId.Should().Be(technician.Id);
            member.MaxConcurrentTickets.Should().Be(5);
            member.IsActive.Should().BeTrue();
        }

        [Fact]
        public void Should_Reactivate_Member_When_Adding_Previously_Removed_Technician()
        {
            var queue = new SupportQueueBuilder().Build();
            var technician = new UserBuilder()
                .WithRole(SIGTI.Domain.Enums.Role.Technician)
                .Build();
            queue.AddMember(technician, 3);
            queue.RemoveMember(technician);

            queue.AddMember(technician, 7);

            queue.Members.Should().HaveCount(1);
            queue.GetActiveMemberCount().Should().Be(1);
            var member = queue.Members.First();
            member.IsActive.Should().BeTrue();
            member.MaxConcurrentTickets.Should().Be(7);
        }

        [Fact]
        public void Should_Throw_When_Adding_Null_Technician()
        {
            var queue = new SupportQueueBuilder().Build();

            Action action = () => queue.AddMember(null!, 5);

            action
                .Should()
                .Throw<DomainException>()
                .WithMessage("O técnico é obrigatório.");
        }

        [Fact]
        public void Should_Throw_When_Adding_Non_Technician_User()
        {
            var queue = new SupportQueueBuilder().Build();
            var regularUser = new UserBuilder()
                .WithRole(SIGTI.Domain.Enums.Role.User)
                .Build();

            Action action = () => queue.AddMember(regularUser, 5);

            action
                .Should()
                .Throw<DomainException>()
                .WithMessage(
                    "O usuário deve ser um técnico para ser adicionado à fila."
                );
        }

        [Fact]
        public void Should_Throw_When_Adding_Already_Active_Member()
        {
            var queue = new SupportQueueBuilder().Build();
            var technician = new UserBuilder()
                .WithRole(SIGTI.Domain.Enums.Role.Technician)
                .Build();
            queue.AddMember(technician, 5);

            Action action = () => queue.AddMember(technician, 3);

            action
                .Should()
                .Throw<DomainException>()
                .WithMessage("O técnico já é membro ativo da fila.");
        }

        [Fact]
        public void Should_Remove_Technician_From_Queue()
        {
            var queue = new SupportQueueBuilder().Build();
            var technician = new UserBuilder()
                .WithRole(SIGTI.Domain.Enums.Role.Technician)
                .Build();
            queue.AddMember(technician, 5);

            queue.RemoveMember(technician);

            queue.GetActiveMemberCount().Should().Be(0);
            var member = queue.Members.First();
            member.IsActive.Should().BeFalse();
            member.LeftAt.Should().NotBeNull();
        }

        [Fact]
        public void Should_Throw_When_Removing_Null_Technician()
        {
            var queue = new SupportQueueBuilder().Build();

            Action action = () => queue.RemoveMember(null!);

            action
                .Should()
                .Throw<DomainException>()
                .WithMessage("O técnico é obrigatório.");
        }

        [Fact]
        public void Should_Throw_When_Removing_Non_Technician_User()
        {
            var queue = new SupportQueueBuilder().Build();
            var regularUser = new UserBuilder()
                .WithRole(SIGTI.Domain.Enums.Role.User)
                .Build();

            Action action = () => queue.RemoveMember(regularUser);

            action
                .Should()
                .Throw<DomainException>()
                .WithMessage(
                    "O usuário deve ser um técnico para ser removido da fila."
                );
        }

        [Fact]
        public void Should_Throw_When_Removing_Technician_Not_In_Queue()
        {
            var queue = new SupportQueueBuilder().Build();
            var technician = new UserBuilder()
                .WithRole(SIGTI.Domain.Enums.Role.Technician)
                .Build();

            Action action = () => queue.RemoveMember(technician);

            action
                .Should()
                .Throw<DomainException>()
                .WithMessage("O técnico não é membro da fila.");
        }

        [Fact]
        public void Should_Update_Member_Capacity_Successfully()
        {
            var queue = new SupportQueueBuilder().Build();
            var technician = new UserBuilder()
                .WithRole(SIGTI.Domain.Enums.Role.Technician)
                .Build();
            queue.AddMember(technician, 3);

            queue.UpdateMemberCapacity(technician, 8);

            var member = queue.Members.First();
            member.MaxConcurrentTickets.Should().Be(8);
            queue.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void Should_Throw_When_Updating_Capacity_With_Null_Technician()
        {
            var queue = new SupportQueueBuilder().Build();

            Action action = () => queue.UpdateMemberCapacity(null!, 5);

            action
                .Should()
                .Throw<DomainException>()
                .WithMessage("O técnico é obrigatório.");
        }

        [Fact]
        public void Should_Throw_When_Updating_Capacity_Of_Non_Member()
        {
            var queue = new SupportQueueBuilder().Build();
            var technician = new UserBuilder()
                .WithRole(SIGTI.Domain.Enums.Role.Technician)
                .Build();

            Action action = () => queue.UpdateMemberCapacity(technician, 5);

            action
                .Should()
                .Throw<DomainException>()
                .WithMessage("O técnico não é membro ativo da fila.");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Should_Throw_When_Updating_Capacity_With_Invalid_Limit(
            int invalidCapacity
        )
        {
            var queue = new SupportQueueBuilder().Build();
            var technician = new UserBuilder()
                .WithRole(SIGTI.Domain.Enums.Role.Technician)
                .Build();
            queue.AddMember(technician, 3);

            Action action = () =>
                queue.UpdateMemberCapacity(technician, invalidCapacity);

            action
                .Should()
                .Throw<DomainException>()
                .WithMessage("A capacidade máxima deve ser maior que zero.");
        }
    }
}

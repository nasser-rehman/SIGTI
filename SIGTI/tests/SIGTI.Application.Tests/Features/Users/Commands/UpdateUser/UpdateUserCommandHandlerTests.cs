using FluentAssertions;
using Moq;
using SIGTI.Application.Common.Exceptions;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Application.Common.Interfaces.Services;
using SIGTI.Application.Features.Users.Commands.UpdateUser;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Enums;
using SIGTI.Domain.Exceptions;
using SIGTI.Domain.Tests.Builders;

namespace SIGTI.Application.Tests.Features.Users.Commands.UpdateUser
{
    public class UpdateUserCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IEntityReferenceService> _entityReferenceServiceMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly UpdateUserCommandHandler _handler;

        public UpdateUserCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _entityReferenceServiceMock = new Mock<IEntityReferenceService>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new UpdateUserCommandHandler(
                _userRepositoryMock.Object,
                _entityReferenceServiceMock.Object,
                _unitOfWorkMock.Object
            );
        }

        [Fact]
        public async Task Handle_WhenUserAndDepartmentExist_ShouldUpdateUserAndCommit()
        {
            // Arrange
            var user = new UserBuilder().Build();
            var department = new DepartmentBuilder().Build();

            _userRepositoryMock
                .Setup(r =>
                    r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(user);

            _entityReferenceServiceMock
                .Setup(s =>
                    s.GetRequiredDepartmentAsync(
                        department.Id,
                        It.IsAny<CancellationToken>()
                    )
                )
                .ReturnsAsync(department);

            var newName = "This is a new Name to user";

            var command = new UpdateUserCommand(
                user.Id,
                newName,
                Role.User,
                department.Id
            );

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().NotBeEmpty();
            result.Name.Should().Be(newName);
            result.Role.Should().Be(Role.User);
            result.DepartmentId.Should().Be(department.Id);

            _unitOfWorkMock.Verify(
                uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task Handle_WhenUserDoesNotExist_ShouldThrowNotFoundException()
        {
            // Arrange
            var nonExistentUser = Guid.Empty;

            _userRepositoryMock
                .Setup(r =>
                    r.GetByIdAsync(
                        nonExistentUser,
                        It.IsAny<CancellationToken>()
                    )
                )
                .ReturnsAsync((User?)null);

            var command = new UpdateUserCommand(
                nonExistentUser,
                "New name to this",
                Role.Technician,
                Guid.NewGuid()
            );

            // Act
            var act = () => _handler.Handle(command, CancellationToken.None);

            // Assert

            await act.Should().ThrowAsync<NotFoundException>();
            _unitOfWorkMock.Verify(
                uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task Handle_WhenDepartmentDoesNotExist_ShouldThrowNotFoundException()
        {
            var user = new UserBuilder().Build();
            var nonExistentDepartment = Guid.NewGuid();

            _userRepositoryMock
                .Setup(r =>
                    r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(user);

            _entityReferenceServiceMock
                .Setup(s =>
                    s.GetRequiredDepartmentAsync(
                        nonExistentDepartment,
                        It.IsAny<CancellationToken>()
                    )
                )
                .ThrowsAsync(
                    new NotFoundException(
                        nameof(Department),
                        nonExistentDepartment
                    )
                );

            var command = new UpdateUserCommand(
                user.Id,
                user.Name,
                user.Role,
                nonExistentDepartment
            );

            // Act
            var act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();

            _unitOfWorkMock.Verify(
                uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task Handle_WhenTargetUserIsSystemAndRoleChanges_ShouldThrowDomainException()
        {
            var user = new UserBuilder().Build();
            // Set as SystemUser
            typeof(User)
                .GetProperty(nameof(User.IsSystem))!
                .SetValue(user, true);

            _userRepositoryMock
                .Setup(r =>
                    r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(user);

            var command = new UpdateUserCommand(
                user.Id,
                user.Name,
                Role.User,
                user.DepartmentId
            );

            // Act
            var act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<DomainException>()
                .WithMessage(
                    "Não é possível alterar a função de um usuário System."
                );

            _unitOfWorkMock.Verify(
                uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }
    }
}

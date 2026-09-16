using FluentAssertions;
using Moq;
using SIGTI.Application.Common.Exceptions;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Application.Common.Interfaces.Services;
using SIGTI.Application.Features.Users.Commands.DeactivateUser;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Enums;
using SIGTI.Domain.Exceptions;
using SIGTI.Domain.Tests.Builders;

namespace SIGTI.Application.Tests.Features.Users.Commands.DeactivateUser
{
    public class DeactivateUserCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly DeactivateUserCommandHandler _handler;

        public DeactivateUserCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();

            _handler = new DeactivateUserCommandHandler(
                _userRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _currentUserServiceMock.Object
            );
        }

        [Fact]
        public async Task Handle_WhenUserIsValid_ShouldDeactivateUserAndCommit()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            _currentUserServiceMock.Setup(s => s.UserId).Returns(adminId);

            var user = new UserBuilder()
                .WithName("Test User Name")
                .WithEmail("user@sigti.local")
                .WithRole(Role.Technician)
                .Build();

            _userRepositoryMock
                .Setup(r =>
                    r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(user);

            var command = new DeactivateUserCommand(user.Id);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(user.Id);
            result.IsActive.Should().BeFalse();

            user.IsActive.Should().BeFalse();

            _unitOfWorkMock.Verify(
                uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task Handle_WhenUserNotFound_ShouldThrowNotFoundException()
        {
            // Arrange
            var nonExistentUserId = Guid.NewGuid();
            _userRepositoryMock
                .Setup(r =>
                    r.GetByIdAsync(
                        nonExistentUserId,
                        It.IsAny<CancellationToken>()
                    )
                )
                .ReturnsAsync((User?)null);

            var command = new DeactivateUserCommand(nonExistentUserId);

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
        public async Task Handle_WhenUserIsSystem_ShouldThrowDomainException()
        {
            // Arrange
            var systemUser = new UserBuilder()
                .WithName("System SIGTI")
                .WithEmail("system@sigti.local")
                .Build();

            typeof(User)
                .GetProperty(nameof(User.IsSystem))!
                .SetValue(systemUser, true);

            _userRepositoryMock
                .Setup(r =>
                    r.GetByIdAsync(systemUser.Id, It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(systemUser);

            var command = new DeactivateUserCommand(systemUser.Id);

            // Act
            var act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<DomainException>()
                .WithMessage("Não é possível desativar um usuário do sistema.");

            _unitOfWorkMock.Verify(
                uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task Handle_WhenAdminAttemptsToDeactivateSelf_ShouldThrowDomainException()
        {
            // Arrange
            var adminUser = new UserBuilder()
                .WithName("administrator")
                .WithEmail("admin@sigit.local")
                .WithRole(Role.Administrator)
                .Build();

            _currentUserServiceMock.Setup(s => s.UserId).Returns(adminUser.Id);

            _userRepositoryMock
                .Setup(r =>
                    r.GetByIdAsync(adminUser.Id, It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(adminUser);

            var command = new DeactivateUserCommand(adminUser.Id);

            // Act
            var act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<DomainException>()
                .WithMessage(
                    "O administrador não pode desativar o próprio usuário."
                );

            _unitOfWorkMock.Verify(
                uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task Handle_WhenUserIsAlreadyInactive_ShouldThrowDomainException()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            _currentUserServiceMock.Setup(s => s.UserId).Returns(adminId);

            var user = new UserBuilder()
                .WithName("User test name")
                .WithEmail("user@sigti.local")
                .AsDeactivated()
                .Build();

            _userRepositoryMock
                .Setup(r =>
                    r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(user);

            var command = new DeactivateUserCommand(user.Id);

            // Act
            var act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<DomainException>()
                .WithMessage("O usuário já está desativado.");

            _unitOfWorkMock.Verify(
                uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }
    }
}

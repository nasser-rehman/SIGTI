using FluentAssertions;
using Moq;
using SIGTI.Application.Common.Exceptions;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Application.Common.Interfaces.Services;
using SIGTI.Application.Features.Users.Commands.ChangePassword;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Enums;
using SIGTI.Domain.Exceptions;
using SIGTI.Domain.ValueObjects;

namespace SIGTI.Application.Tests.Features.Users.Commands.ChangePassword
{
    public class ChangePasswordCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock = new();
        private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly ChangePasswordCommandHandler _handler;

        public ChangePasswordCommandHandlerTests()
        {
            _handler = new ChangePasswordCommandHandler(
                _userRepositoryMock.Object,
                _passwordHasherMock.Object,
                _unitOfWorkMock.Object
            );
        }

        [Fact]
        public async Task Handle_ValidRequest_ShouldUpdatePasswordHashAndSave()
        {
            // Arrange
            var dept = new Department("TI", "Departamento de TI");
            var user = new User(
                "Carlos Silva",
                new Email("carlos@sigti.local"),
                "hashedCurrentPassword",
                Role.User,
                dept
            );

            _userRepositoryMock
                .Setup(r =>
                    r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(user);

            _passwordHasherMock
                .Setup(h =>
                    h.Verify("CurrentPassword123", "hashedCurrentPassword")
                )
                .Returns(true);

            _passwordHasherMock
                .Setup(h => h.Hash("NewPassword123"))
                .Returns("hashedNewPassword");

            var command = new ChangePasswordCommand(
                user.Id,
                "CurrentPassword123",
                "NewPassword123"
            );

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            user.PasswordHash.Should().Be("hashedNewPassword");
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task Handle_UserNotFound_ShouldThrowNotFoundException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _userRepositoryMock
                .Setup(r =>
                    r.GetByIdAsync(userId, It.IsAny<CancellationToken>())
                )
                .ReturnsAsync((User?)null);

            var command = new ChangePasswordCommand(
                userId,
                "CurrentPassword123",
                "NewPassword123"
            );

            // Act
            var act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task Handle_IncorrectCurrentPassword_ShouldThrowDomainException()
        {
            // Arrange
            var dept = new Department("TI", "Departamento de TI");
            var user = new User(
                "Carlos Silva",
                new Email("carlos@sigti.local"),
                "hashedCurrentPassword",
                Role.User,
                dept
            );

            _userRepositoryMock
                .Setup(r =>
                    r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(user);

            _passwordHasherMock
                .Setup(h =>
                    h.Verify("WrongPassword123", "hashedCurrentPassword")
                )
                .Returns(false);

            var command = new ChangePasswordCommand(
                user.Id,
                "WrongPassword123",
                "NewPassword123"
            );

            // Act
            var act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<DomainException>()
                .WithMessage("A senha atual informada está incorreta.");
            _unitOfWorkMock.Verify(
                u => u.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }
    }
}

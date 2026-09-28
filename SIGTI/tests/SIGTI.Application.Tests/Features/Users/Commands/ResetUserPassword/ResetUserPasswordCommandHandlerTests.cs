using FluentAssertions;
using Moq;
using SIGTI.Application.Common.Exceptions;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Application.Common.Interfaces.Services;
using SIGTI.Application.Features.Users.Commands.ResetUserPassword;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Enums;
using SIGTI.Domain.ValueObjects;

namespace SIGTI.Application.Tests.Features.Users.Commands.ResetUserPassword
{
    public class ResetUserPasswordCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock = new();
        private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly ResetUserPasswordCommandHandler _handler;

        public ResetUserPasswordCommandHandlerTests()
        {
            _handler = new ResetUserPasswordCommandHandler(
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
                "oldHashedPassword",
                Role.User,
                dept
            );

            _userRepositoryMock
                .Setup(r =>
                    r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(user);

            _passwordHasherMock
                .Setup(h => h.Hash("ResetPassword123"))
                .Returns("newHashedPassword");

            var command = new ResetUserPasswordCommand(
                user.Id,
                "ResetPassword123"
            );

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            user.PasswordHash.Should().Be("newHashedPassword");
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

            var command = new ResetUserPasswordCommand(
                userId,
                "ResetPassword123"
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
    }
}

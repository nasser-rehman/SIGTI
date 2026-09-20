using FluentAssertions;
using Moq;
using SIGTI.Application.Common.Exceptions;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Application.Features.Users.Commands.ActivateUser;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Exceptions;
using SIGTI.Domain.Tests.Builders;

namespace SIGTI.Application.Tests.Features.Users.Commands.ActivateUser
{
    public class ActivateUserCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly ActivateUserCommandHandler _handler;

        public ActivateUserCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new ActivateUserCommandHandler(
                _userRepositoryMock.Object,
                _unitOfWorkMock.Object
            );
        }

        [Fact]
        public async Task Handle_WhenUserExistsAndIsDeactivated_ShouldActivateUserAndCommit()
        {
            // Arrange
            var user = new UserBuilder().AsDeactivated().Build();
            _userRepositoryMock
                .Setup(r =>
                    r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(user);

            var command = new ActivateUserCommand(user.Id);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().NotBeEmpty();
            result.IsActive.Should().BeTrue();

            _unitOfWorkMock.Verify(
                uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task Handle_WhenUserDoesNotExist_ShouldThrowNotFoundException()
        {
            // Arrange
            var nonExistentId = Guid.Empty;
            _userRepositoryMock
                .Setup(r =>
                    r.GetByIdAsync(nonExistentId, It.IsAny<CancellationToken>())
                )
                .ReturnsAsync((User?)null);

            var command = new ActivateUserCommand(nonExistentId);

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
        public async Task Handle_WhenUserIsAlreadyActive_ShouldThrowDomainException()
        {
            // Arrange
            var user = new UserBuilder().Build();
            _userRepositoryMock
                .Setup(r =>
                    r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(user);

            var command = new ActivateUserCommand(user.Id);

            // Act
            var act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<DomainException>()
                .WithMessage("O usuário já está ativado.");
            _unitOfWorkMock.Verify(
                uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }
    }
}

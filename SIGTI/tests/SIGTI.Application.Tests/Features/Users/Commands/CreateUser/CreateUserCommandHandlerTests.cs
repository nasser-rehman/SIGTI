using FluentAssertions;
using Moq;
using SIGTI.Application.Common.Exceptions;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Application.Common.Interfaces.Services;
using SIGTI.Application.Features.Users.Commands.CreateUser;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Enums;
using SIGTI.Domain.Exceptions;
using SIGTI.Domain.Tests.Builders;
using SIGTI.Domain.ValueObjects;

namespace SIGTI.Application.Tests.Features.Users.Commands.CreateUser
{
    public class CreateUserCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IEntityReferenceService> _entityReferenceServiceMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly CreateUserCommandHandler _handler;

        public CreateUserCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _entityReferenceServiceMock = new Mock<IEntityReferenceService>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _handler = new CreateUserCommandHandler(
                _userRepositoryMock.Object,
                _entityReferenceServiceMock.Object,
                _passwordHasherMock.Object,
                _unitOfWorkMock.Object
            );
        }

        [Fact]
        public async Task Handle_WhenCommandIsValid_ShouldCreateUserAndCommit()
        {
            // Arrange
            var department = new DepartmentBuilder().Build();

            _userRepositoryMock
                .Setup(r =>
                    r.ExistsByEmailASync(
                        It.IsAny<Email>(),
                        It.IsAny<CancellationToken>()
                    )
                )
                .ReturnsAsync(false);

            _entityReferenceServiceMock
                .Setup(s =>
                    s.GetRequiredDepartmentAsync(
                        department.Id,
                        It.IsAny<CancellationToken>()
                    )
                )
                .ReturnsAsync(department);

            _passwordHasherMock
                .Setup(h => h.Hash("StrongPass@123"))
                .Returns("hashed_bcrypt_password");

            var command = new CreateUserCommand(
                "Nasser Rehman",
                "nasser@sigti.local",
                "StrongPass@123",
                Role.Technician,
                department.Id
            );

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().NotBeEmpty();
            result.Name.Should().Be("Nasser Rehman");
            result.Email.Should().Be("nasser@sigti.local");
            result.Role.Should().Be(Role.Technician);
            result.DepartmentId.Should().Be(department.Id);
            result.IsActive.Should().BeTrue();

            _passwordHasherMock.Verify(
                h => h.Hash("StrongPass@123"),
                Times.Once
            );

            _userRepositoryMock.Verify(
                r =>
                    r.AddAsync(
                        It.Is<User>(user =>
                            user.Name == "Nasser Rehman"
                            && user.PasswordHash == "hashed_bcrypt_password"
                            && user.Role == Role.Technician
                        ),
                        It.IsAny<CancellationToken>()
                    ),
                Times.Once
            );

            _unitOfWorkMock.Verify(
                uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task Handle_WhenEmailAlreadyExists_ShouldThrowDomainException()
        {
            // Arrage
            _userRepositoryMock
                .Setup(r =>
                    r.ExistsByEmailASync(
                        It.IsAny<Email>(),
                        It.IsAny<CancellationToken>()
                    )
                )
                .ReturnsAsync(true);

            var command = new CreateUserCommand(
                "Nasser Rehman",
                "already_exists@sigti.com",
                "StrongPass@123",
                Role.Technician,
                Guid.NewGuid()
            );

            // Act
            var act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<DomainException>()
                .WithMessage("*Já existe um usuário cadastrado com o e-mail*");

            _userRepositoryMock.Verify(
                r =>
                    r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
                Times.Never
            );

            _unitOfWorkMock.Verify(
                uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task Handle_WhenDepartmentNotFound_ShouldThrowNotFoundException()
        {
            // Arrange
            var nonExistentDepartmentId = Guid.NewGuid();

            _userRepositoryMock
                .Setup(r =>
                    r.ExistsByEmailASync(
                        It.IsAny<Email>(),
                        It.IsAny<CancellationToken>()
                    )
                )
                .ReturnsAsync(false);

            _entityReferenceServiceMock
                .Setup(s =>
                    s.GetRequiredDepartmentAsync(
                        nonExistentDepartmentId,
                        It.IsAny<CancellationToken>()
                    )
                )
                .ThrowsAsync(
                    new NotFoundException(
                        nameof(Department),
                        nonExistentDepartmentId
                    )
                );

            var command = new CreateUserCommand(
                "Nasser Rehman",
                "nasser@sigti.local",
                "StrongPass@123",
                Role.Technician,
                nonExistentDepartmentId
            );

            // Act
            var act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();

            _userRepositoryMock.Verify(
                r =>
                    r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
                Times.Never
            );

            _unitOfWorkMock.Verify(
                uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }
    }
}

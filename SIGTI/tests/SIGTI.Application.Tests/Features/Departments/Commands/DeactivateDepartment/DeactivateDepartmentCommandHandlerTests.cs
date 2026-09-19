using FluentAssertions;
using Moq;
using SIGTI.Application.Common.Exceptions;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Application.Features.Departments.Commands.DeactivateDepartment;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Exceptions;
using SIGTI.Domain.Tests.Builders;

namespace SIGTI.Application.Tests.Features.Departments.Commands.DeactivateDepartment
{
    public class DeactivateDepartmentCommandHandlerTests
    {
        private readonly Mock<IDepartmentRepository> _departmentRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly DeactivateDepartmentCommandHandler _handler;

        public DeactivateDepartmentCommandHandlerTests()
        {
            _departmentRepositoryMock = new Mock<IDepartmentRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new DeactivateDepartmentCommandHandler(
                _departmentRepositoryMock.Object,
                _unitOfWorkMock.Object
            );
        }

        [Fact]
        public async Task Handle_WhenDepartmentIdExistAndIsActivated_ShouldDisableDepartmentAndCommit()
        {
            // Arrange
            var department = new DepartmentBuilder().Build();

            _departmentRepositoryMock
                .Setup(r =>
                    r.GetByIdAsync(department.Id, It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(department);

            var command = new DeactivateDepartmentCommand(department.Id);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().NotBeEmpty();
            result.IsActive.Should().BeFalse();

            _unitOfWorkMock.Verify(
                uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task Handle_WhenDepartmentDoesNotExist_ShouldThrowNotFoundException()
        {
            // Arrange
            var nonExistentDepartmentId = Guid.NewGuid();

            _departmentRepositoryMock
                .Setup(r =>
                    r.GetByIdAsync(
                        nonExistentDepartmentId,
                        It.IsAny<CancellationToken>()
                    )
                )
                .ReturnsAsync((Department?)null);

            var command = new DeactivateDepartmentCommand(
                nonExistentDepartmentId
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
        public async Task Handle_WhenDepartmentAlreadyDeactivated_shouldThrowDomainException()
        {
            // Arrange
            var department = new DepartmentBuilder().AsDeactivated().Build();

            _departmentRepositoryMock
                .Setup(r =>
                    r.GetByIdAsync(department.Id, It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(department);

            var command = new DeactivateDepartmentCommand(department.Id);

            // Act
            var act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainException>();

            _unitOfWorkMock.Verify(
                uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }
    }
}

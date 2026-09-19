using FluentAssertions;
using Moq;
using SIGTI.Application.Common.Exceptions;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Application.Features.Departments.Commands.ActivateDepartment;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Exceptions;
using SIGTI.Domain.Tests.Builders;

namespace SIGTI.Application.Tests.Features.Departments.Commands.ActivateDepartment
{
    public class ActivateDepartmentCommandHandlerTests
    {
        private readonly Mock<IDepartmentRepository> _departmentRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly ActivateDepartmentCommandHandler _handler;

        public ActivateDepartmentCommandHandlerTests()
        {
            _departmentRepositoryMock = new Mock<IDepartmentRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new ActivateDepartmentCommandHandler(
                _departmentRepositoryMock.Object,
                _unitOfWorkMock.Object
            );
        }

        [Fact]
        public async Task Handle_WhenDepartmentIdExistAndIsDeactivated_ShouldActivateDepartmentAndCommit()
        {
            var department = new DepartmentBuilder().AsDeactivated().Build();

            _departmentRepositoryMock
                .Setup(r =>
                    r.GetByIdAsync(department.Id, It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(department);

            var command = new ActivateDepartmentCommand(department.Id);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().NotBeNull();
            result.Id.Should().Be(department.Id);
            result.IsActive.Should().BeTrue();
            result.UpdatedAt.Should().Be(department.UpdatedAt);

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

            var command = new ActivateDepartmentCommand(
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
        public async Task Handle_WhenDepartmentAlreadyActivated_shouldThrowDomainException()
        {
            // Arrange
            var department = new DepartmentBuilder().Build();

            _departmentRepositoryMock
                .Setup(r =>
                    r.GetByIdAsync(department.Id, It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(department);

            var command = new ActivateDepartmentCommand(department.Id);

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

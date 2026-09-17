using FluentAssertions;
using Moq;
using SIGTI.Application.Common.Exceptions;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Application.Features.Departments.Commands.UpdateDepartment;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Exceptions;
using SIGTI.Domain.Tests.Builders;

namespace SIGTI.Application.Tests.Features.Departments.Commands.UpdateDepartment
{
    public class UpdateDepartmentCommandHandlerTests
    {
        private readonly Mock<IDepartmentRepository> _departmentRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly UpdateDepartmentCommandHandler _handler;

        public UpdateDepartmentCommandHandlerTests()
        {
            _departmentRepositoryMock = new Mock<IDepartmentRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new UpdateDepartmentCommandHandler(
                _departmentRepositoryMock.Object,
                _unitOfWorkMock.Object
            );
        }

        [Fact]
        public async Task Handle_WhenDepartmentExistAndNewNameNotInUseIsGiven_ShouldUpdateNameAndCommit()
        {
            // Arrange
            var department = new DepartmentBuilder()
                .WithName("Name to change")
                .WithDescription("Good description for a department here.")
                .Build();

            _departmentRepositoryMock
                .Setup(r =>
                    r.GetByIdAsync(department.Id, It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(department);

            var newName = "New Good Name";

            _departmentRepositoryMock
                .Setup(r =>
                    r.ExistsByNameAsync(newName, It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(false);

            var command = new UpdateDepartmentCommand(
                department.Id,
                newName,
                department.Description
            );

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(department.Id);
            result.Name.Should().Be("New Good Name");
            result.Description.Should().Be(department.Description);

            _departmentRepositoryMock.Verify(
                r =>
                    r.GetByIdAsync(
                        department.Id,
                        It.IsAny<CancellationToken>()
                    ),
                Times.Once
            );
            _departmentRepositoryMock.Verify(
                r =>
                    r.ExistsByNameAsync(newName, It.IsAny<CancellationToken>()),
                Times.Once
            );
            _unitOfWorkMock.Verify(
                uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task Handle_WhenDepartmentExistAndNewDescriptionIsProvided_ShouldUpdateDepartmentAndCommit()
        {
            // Arrange
            var department = new DepartmentBuilder().Build();

            _departmentRepositoryMock
                .Setup(r =>
                    r.GetByIdAsync(department.Id, It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(department);

            var newDescripton = "New Good Description to Save";

            var command = new UpdateDepartmentCommand(
                department.Id,
                department.Name,
                newDescripton
            );

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(department.Id);
            result.Name.Should().Be(department.Name);
            result.Description.Should().Be(newDescripton);

            _departmentRepositoryMock.Verify(
                r =>
                    r.GetByIdAsync(
                        department.Id,
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
        public async Task Handle_WhenDepartmentNotExist_ShouldThrowNotFoundException()
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

            var command = new UpdateDepartmentCommand(
                nonExistentDepartmentId,
                "Not gonna save",
                "Not gonna save"
            );

            // act
            var act = () => _handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task Handle_WhenDepartmentNameAlreadyExist_ShouldThrowDomainException()
        {
            // Arrange
            var department = new DepartmentBuilder()
                .WithName("Nome Atual")
                .Build();
            var duplicateName = "Nome Já Existente";

            _departmentRepositoryMock
                .Setup(r =>
                    r.GetByIdAsync(department.Id, It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(department);

            _departmentRepositoryMock
                .Setup(r =>
                    r.ExistsByNameAsync(
                        duplicateName,
                        It.IsAny<CancellationToken>()
                    )
                )
                .ReturnsAsync(true);

            var command = new UpdateDepartmentCommand(
                department.Id,
                duplicateName,
                department.Description
            );

            // Act
            var act = () => _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                .ThrowAsync<DomainException>()
                .WithMessage("Departamento com o mesmo nome já existente.");
        }
    }
}

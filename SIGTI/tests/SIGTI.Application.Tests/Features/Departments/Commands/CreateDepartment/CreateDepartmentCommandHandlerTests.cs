using FluentAssertions;
using Moq;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Application.Features.Departments.Commands.CreateDepartment;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Exceptions;
using SIGTI.Domain.Tests.Builders;

namespace SIGTI.Application.Tests.Features.Departments.Commands.CreateDepartment
{
    public class CreateDepartmentCommandHandlerTests
    {
        private readonly Mock<IDepartmentRepository> _departmentRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly CreateDepartmentCommandHandler _handler;

        public CreateDepartmentCommandHandlerTests()
        {
            _departmentRepositoryMock = new Mock<IDepartmentRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new CreateDepartmentCommandHandler(
                _departmentRepositoryMock.Object,
                _unitOfWorkMock.Object
            );
        }

        [Fact]
        public async Task Handle_WhenDataIsValid_ShouldCreateDepartmentAndCommit()
        {
            var department = new DepartmentBuilder()
                .WithName("Test department")
                .Build();

            _departmentRepositoryMock
                .Setup(x =>
                    x.ExistsByNameAsync(department.Name, CancellationToken.None)
                )
                .ReturnsAsync(false);

            var command = new CreateDepartmentCommand(
                department.Name,
                department.Description
            );

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().NotBeNull();
            result.Id.Should().NotBeEmpty();
            result.Name.Should().Be(department.Name);
            result.Description.Should().Be(department.Description);
            result.IsActive.Should().Be(department.IsActive);

            _departmentRepositoryMock.Verify(
                rep =>
                    rep.AddAsync(
                        It.IsAny<Department>(),
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
        public async Task Handle_WhenDepartmentNameAlreadyExists_ShouldThrowDomainException()
        {
            var department = new DepartmentBuilder().Build();

            _departmentRepositoryMock
                .Setup(x =>
                    x.ExistsByNameAsync(
                        department.Name,
                        It.IsAny<CancellationToken>()
                    )
                )
                .ReturnsAsync(true);

            var command = new CreateDepartmentCommand(
                department.Name,
                department.Description
            );

            var act = () => _handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<DomainException>();

            _departmentRepositoryMock.Verify(
                rep =>
                    rep.AddAsync(
                        It.IsAny<Department>(),
                        It.IsAny<CancellationToken>()
                    ),
                Times.Never
            );

            _unitOfWorkMock.Verify(
                uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never
            );
        }
    }
}

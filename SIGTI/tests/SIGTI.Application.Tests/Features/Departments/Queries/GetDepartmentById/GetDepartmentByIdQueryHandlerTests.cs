using FluentAssertions;
using Moq;
using SIGTI.Application.Common.Exceptions;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Application.Features.Departments.Commands.UpdateDepartment;
using SIGTI.Application.Features.Departments.Queries.GetDepartmentById;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Tests.Builders;

namespace SIGTI.Application.Tests.Features.Departments.Queries.GetDepartmentById
{
    public class GetDepartmentByIdQueryHandlerTests
    {
        private readonly Mock<IDepartmentRepository> _departmentRepositoryMock;

        private readonly GetDepartmentByIdQueryHandler _handler;

        public GetDepartmentByIdQueryHandlerTests()
        {
            _departmentRepositoryMock = new Mock<IDepartmentRepository>();
            _handler = new GetDepartmentByIdQueryHandler(
                _departmentRepositoryMock.Object
            );
        }

        [Fact]
        public async Task Handle_WhenDepartmentExist_ShouldReturnDepartmentDetails()
        {
            // Arrange
            var department = new DepartmentBuilder().Build();

            _departmentRepositoryMock
                .Setup(r =>
                    r.GetByIdAsync(department.Id, It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(department);

            var query = new GetDepartmentByIdQuery(department.Id);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().NotBeEmpty();
            result.Name.Should().Be(department.Name);
            result.Description.Should().Be(department.Description);
            result.CreatedAt.Should().Be(department.CreatedAt);
            result.IsActive.Should().BeTrue();

            _departmentRepositoryMock.Verify(
                r =>
                    r.GetByIdAsync(
                        department.Id,
                        It.IsAny<CancellationToken>()
                    ),
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

            var query = new GetDepartmentByIdQuery(nonExistentDepartmentId);

            // Act
            var act = () => _handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}

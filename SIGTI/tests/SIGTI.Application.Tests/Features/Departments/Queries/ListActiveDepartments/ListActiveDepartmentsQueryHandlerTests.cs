using FluentAssertions;
using Moq;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Application.Features.Departments.Queries.ListActiveDepartments;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Tests.Builders;

namespace SIGTI.Application.Tests.Features.Departments.Queries.ListActiveDepartments
{
    public class ListActiveDepartmentsQueryHandlerTests
    {
        private readonly Mock<IDepartmentRepository> _departmentRepositoryMock;
        private readonly ListActiveDepartmentsQueryHandler _handler;

        public ListActiveDepartmentsQueryHandlerTests()
        {
            _departmentRepositoryMock = new Mock<IDepartmentRepository>();
            _handler = new ListActiveDepartmentsQueryHandler(
                _departmentRepositoryMock.Object
            );
        }

        [Fact]
        public async Task Handle_WhenActiveDepartmentsExists_ShouldReturnMappedResponses()
        {
            var departmentOne = new DepartmentBuilder()
                .WithName("Department One")
                .Build();
            var departmentTwo = new DepartmentBuilder()
                .WithName("Department Two")
                .Build();

            _departmentRepositoryMock
                .Setup(r => r.ListActiveAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    new List<Department> { departmentOne, departmentTwo }
                );

            var result = await _handler.Handle(
                new ListActiveDepartmentsQuery(),
                CancellationToken.None
            );

            result.Should().HaveCount(2);
            result
                .Should()
                .Contain(department =>
                    department.Id == departmentOne.Id
                    && department.Name == departmentOne.Name
                );
        }
    }
}

using MediatR;
using SIGTI.Application.Common.Interfaces.Persistence;

namespace SIGTI.Application.Features.Departments.Queries.ListActiveDepartments
{
    public sealed class ListActiveDepartmentsQueryHandler
        : IRequestHandler<
            ListActiveDepartmentsQuery,
            IReadOnlyCollection<ListActiveDepartmentsResponse>
        >
    {
        private readonly IDepartmentRepository _departmentRepository;

        public ListActiveDepartmentsQueryHandler(
            IDepartmentRepository departmentRepository
        )
        {
            _departmentRepository = departmentRepository;
        }

        public async Task<
            IReadOnlyCollection<ListActiveDepartmentsResponse>
        > Handle(
            ListActiveDepartmentsQuery request,
            CancellationToken cancellationToken
        )
        {
            var departments = await _departmentRepository.ListActiveAsync(
                cancellationToken
            );

            return departments
                .Select(department => new ListActiveDepartmentsResponse(
                    department.Id,
                    department.Name,
                    department.Description,
                    department.IsActive
                ))
                .ToList();
        }
    }
}

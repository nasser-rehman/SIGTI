using MediatR;
using SIGTI.Application.Common.Exceptions;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Domain.Entities;

namespace SIGTI.Application.Features.Departments.Queries.GetDepartmentById
{
    public sealed class GetDepartmentByIdQueryHandler
        : IRequestHandler<GetDepartmentByIdQuery, GetDepartmentByIdResponse>
    {
        private readonly IDepartmentRepository _departmentRepository;

        public GetDepartmentByIdQueryHandler(
            IDepartmentRepository departmentRepository
        )
        {
            _departmentRepository = departmentRepository;
        }

        public async Task<GetDepartmentByIdResponse> Handle(
            GetDepartmentByIdQuery request,
            CancellationToken cancellationToken
        )
        {
            var department = await _departmentRepository.GetByIdAsync(
                request.Id,
                cancellationToken
            );

            if (department is null)
            {
                throw new NotFoundException(nameof(Department), request.Id);
            }

            return new GetDepartmentByIdResponse(
                department.Id,
                department.Name,
                department.Description,
                department.IsActive,
                department.CreatedAt,
                department.UpdatedAt
            );
        }
    }
}

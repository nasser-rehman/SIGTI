using MediatR;
using SIGTI.Application.Common.Exceptions;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Exceptions;

namespace SIGTI.Application.Features.Departments.Commands.UpdateDepartment
{
    public sealed class UpdateDepartmentCommandHandler
        : IRequestHandler<UpdateDepartmentCommand, UpdateDepartmentResponse>
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateDepartmentCommandHandler(
            IDepartmentRepository departmentRepository,
            IUnitOfWork unitOfWork
        )
        {
            _departmentRepository = departmentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<UpdateDepartmentResponse> Handle(
            UpdateDepartmentCommand request,
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

            if (
                !string.Equals(
                    department.Name,
                    request.Name.Trim(),
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                var nameExists = await _departmentRepository.ExistsByNameAsync(
                    request.Name.Trim(),
                    cancellationToken
                );

                if (nameExists)
                {
                    throw new DomainException(
                        "Departamento com o mesmo nome já existente."
                    );
                }
            }

            department.Update(request.Name, request.Description);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new UpdateDepartmentResponse(
                department.Id,
                department.Name,
                department.Description,
                department.IsActive,
                department.UpdatedAt
            );
        }
    }
}

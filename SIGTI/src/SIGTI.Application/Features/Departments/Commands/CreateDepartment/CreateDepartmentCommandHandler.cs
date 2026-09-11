using MediatR;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Exceptions;

namespace SIGTI.Application.Features.Departments.Commands.CreateDepartment
{
    public sealed class CreateDepartmentCommandHandler
        : IRequestHandler<CreateDepartmentCommand, CreateDepartmentResponse>
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateDepartmentCommandHandler(
            IDepartmentRepository departmentRepository,
            IUnitOfWork unitOfWork
        )
        {
            _departmentRepository = departmentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateDepartmentResponse> Handle(
            CreateDepartmentCommand request,
            CancellationToken cancellationToken
        )
        {
            if (
                await _departmentRepository.ExistsByNameAsync(
                    request.Name,
                    cancellationToken
                )
            )
            {
                throw new DomainException(
                    "Departamento com o mesmo nome já existente."
                );
            }

            var department = new Department(request.Name, request.Description);
            await _departmentRepository.AddAsync(department, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CreateDepartmentResponse(
                department.Id,
                department.Name,
                department.Description,
                department.IsActive
            );
        }
    }
}

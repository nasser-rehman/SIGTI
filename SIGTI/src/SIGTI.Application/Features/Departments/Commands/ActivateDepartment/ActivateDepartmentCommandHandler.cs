using MediatR;
using SIGTI.Application.Common.Exceptions;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Domain.Entities;

namespace SIGTI.Application.Features.Departments.Commands.ActivateDepartment
{
    public sealed class ActivateDepartmentCommandHandler
        : IRequestHandler<ActivateDepartmentCommand, ActivateDepartmentResponse>
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ActivateDepartmentCommandHandler(
            IDepartmentRepository departmentRepository,
            IUnitOfWork unitOfWork
        )
        {
            _departmentRepository = departmentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ActivateDepartmentResponse> Handle(
            ActivateDepartmentCommand request,
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

            department.Activate();
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new ActivateDepartmentResponse(
                department.Id,
                department.IsActive,
                department.UpdatedAt
            );
        }
    }
}

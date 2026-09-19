using MediatR;
using SIGTI.Application.Common.Exceptions;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Domain.Entities;

namespace SIGTI.Application.Features.Departments.Commands.DeactivateDepartment
{
    public sealed class DeactivateDepartmentCommandHandler
        : IRequestHandler<
            DeactivateDepartmentCommand,
            DeactivateDepartmentResponse
        >
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeactivateDepartmentCommandHandler(
            IDepartmentRepository departmentRepository,
            IUnitOfWork unitOfWork
        )
        {
            _departmentRepository = departmentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<DeactivateDepartmentResponse> Handle(
            DeactivateDepartmentCommand request,
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

            department.Deactivate();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new DeactivateDepartmentResponse(
                department.Id,
                department.IsActive,
                department.UpdatedAt
            );
        }
    }
}

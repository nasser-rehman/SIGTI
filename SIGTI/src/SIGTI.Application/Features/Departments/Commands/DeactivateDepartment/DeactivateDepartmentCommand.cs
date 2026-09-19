using MediatR;

namespace SIGTI.Application.Features.Departments.Commands.DeactivateDepartment
{
    public sealed record DeactivateDepartmentCommand(Guid Id)
        : IRequest<DeactivateDepartmentResponse>;
}

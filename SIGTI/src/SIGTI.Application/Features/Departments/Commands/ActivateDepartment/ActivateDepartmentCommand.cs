using MediatR;

namespace SIGTI.Application.Features.Departments.Commands.ActivateDepartment
{
    public sealed record ActivateDepartmentCommand(Guid Id)
        : IRequest<ActivateDepartmentResponse>;
}

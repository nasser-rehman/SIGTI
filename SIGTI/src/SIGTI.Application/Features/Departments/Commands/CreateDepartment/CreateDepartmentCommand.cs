using MediatR;

namespace SIGTI.Application.Features.Departments.Commands.CreateDepartment
{
    public sealed record CreateDepartmentCommand(
        string Name,
        string Description
    ) : IRequest<CreateDepartmentResponse>;
}

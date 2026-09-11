namespace SIGTI.Application.Features.Departments.Commands.CreateDepartment
{
    public sealed record CreateDepartmentResponse(
        Guid Id,
        string Name,
        string Description,
        bool IsActive
    );
}

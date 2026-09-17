namespace SIGTI.Application.Features.Departments.Commands.UpdateDepartment
{
    public sealed record UpdateDepartmentResponse(
        Guid Id,
        string Name,
        string Description,
        bool IsActive,
        DateTime? UpdatedAt
    );
}

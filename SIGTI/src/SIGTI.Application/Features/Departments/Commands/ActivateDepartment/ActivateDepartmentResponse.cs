namespace SIGTI.Application.Features.Departments.Commands.ActivateDepartment
{
    public sealed record ActivateDepartmentResponse(
        Guid Id,
        bool IsActive,
        DateTime? UpdatedAt
    );
}

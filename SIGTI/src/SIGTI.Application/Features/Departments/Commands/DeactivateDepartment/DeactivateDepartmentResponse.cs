namespace SIGTI.Application.Features.Departments.Commands.DeactivateDepartment
{
    public sealed record DeactivateDepartmentResponse(
        Guid Id,
        bool IsActive,
        DateTime? UpdatedAt
    );
}

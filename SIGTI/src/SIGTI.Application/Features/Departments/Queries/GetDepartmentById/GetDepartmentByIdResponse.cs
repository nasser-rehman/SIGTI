namespace SIGTI.Application.Features.Departments.Queries.GetDepartmentById
{
    public sealed record GetDepartmentByIdResponse(
        Guid Id,
        string Name,
        string Description,
        bool IsActive,
        DateTime CreatedAt,
        DateTime? UpdatedAt
    );
}

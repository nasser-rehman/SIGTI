namespace SIGTI.Application.Features.Departments.Queries.ListActiveDepartments
{
    public sealed record ListActiveDepartmentsResponse(
        Guid Id,
        string Name,
        string Description,
        bool IsActive
    );
}

namespace SIGTI.Application.Features.Departments.Commands.CreateDepartment
{
    public sealed record CreateDepartmentRequest(
        string Name,
        string Description
    );
}

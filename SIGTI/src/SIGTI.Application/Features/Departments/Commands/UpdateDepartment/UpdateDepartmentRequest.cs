namespace SIGTI.Application.Features.Departments.Commands.UpdateDepartment
{
    public sealed record UpdateDepartmentRequest(
        string Name,
        string Description
    );
}

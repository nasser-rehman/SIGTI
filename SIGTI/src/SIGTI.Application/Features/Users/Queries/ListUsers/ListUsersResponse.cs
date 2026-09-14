using SIGTI.Domain.Enums;

namespace SIGTI.Application.Features.Users.Queries.ListUsers
{
    public sealed record ListUsersResponse(
        Guid Id,
        string Name,
        string Email,
        Role Role,
        Guid DepartmentId,
        bool IsActive
    );
}

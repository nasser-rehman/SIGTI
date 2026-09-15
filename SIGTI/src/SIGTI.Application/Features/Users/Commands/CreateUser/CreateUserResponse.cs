using SIGTI.Domain.Enums;

namespace SIGTI.Application.Features.Users.Commands.CreateUser
{
    public sealed record CreateUserResponse(
        Guid Id,
        string Name,
        string Email,
        Role Role,
        Guid DepartmentId,
        bool IsActive
    );
}

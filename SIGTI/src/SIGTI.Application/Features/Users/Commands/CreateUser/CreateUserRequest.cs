using SIGTI.Domain.Enums;

namespace SIGTI.Application.Features.Users.Commands.CreateUser
{
    public sealed record CreateUserRequest(
        string Name,
        string Email,
        string Password,
        Role Role,
        Guid DepartmentId
    );
}

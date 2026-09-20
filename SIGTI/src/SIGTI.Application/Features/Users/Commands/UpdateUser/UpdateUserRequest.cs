using SIGTI.Domain.Enums;

namespace SIGTI.Application.Features.Users.Commands.UpdateUser
{
    public sealed record UpdateUserRequest(
        string Name,
        Role Role,
        Guid DepartmentId
    );
}

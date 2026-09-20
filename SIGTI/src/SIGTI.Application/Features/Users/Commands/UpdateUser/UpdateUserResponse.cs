using SIGTI.Domain.Enums;

namespace SIGTI.Application.Features.Users.Commands.UpdateUser
{
    public sealed record UpdateUserResponse(
        Guid Id,
        string Name,
        string Email,
        Role Role,
        Guid DepartmentId,
        bool IsActive,
        DateTime? UpdatedAt
    );
}

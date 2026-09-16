using SIGTI.Domain.Enums;

namespace SIGTI.Application.Features.Users.Queries.GetUserById
{
    public sealed record GetUserByIdResponse(
        Guid Id,
        string Name,
        string Email,
        Role Role,
        Guid DepartmentId,
        bool IsActive,
        DateTime CreatedAt,
        DateTime? UpdatedAt
    );
}

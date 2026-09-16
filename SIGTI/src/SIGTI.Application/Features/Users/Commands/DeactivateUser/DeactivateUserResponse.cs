namespace SIGTI.Application.Features.Users.Commands.DeactivateUser
{
    public sealed record DeactivateUserResponse(
        Guid Id,
        bool IsActive,
        DateTime? UpdatedAt
    );
}

namespace SIGTI.Application.Features.Users.Commands.ActivateUser
{
    public sealed record ActivateUserResponse(
        Guid Id,
        bool IsActive,
        DateTime? UpdatedAt
    );
}

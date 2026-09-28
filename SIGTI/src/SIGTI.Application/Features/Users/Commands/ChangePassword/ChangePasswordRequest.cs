namespace SIGTI.Application.Features.Users.Commands.ChangePassword
{
    public sealed record ChangePasswordRequest(
        string CurrentPassword,
        string NewPassword
    );
}

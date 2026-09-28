using MediatR;

namespace SIGTI.Application.Features.Users.Commands.ResetUserPassword
{
    public sealed record ResetUserPasswordCommand(
        Guid UserId,
        string NewPassword
    ) : IRequest;
}

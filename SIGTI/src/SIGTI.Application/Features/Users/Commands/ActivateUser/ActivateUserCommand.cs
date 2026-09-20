using MediatR;

namespace SIGTI.Application.Features.Users.Commands.ActivateUser
{
    public sealed record ActivateUserCommand(Guid Id)
        : IRequest<ActivateUserResponse>;
}

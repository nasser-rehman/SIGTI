using MediatR;

namespace SIGTI.Application.Features.Users.Commands.DeactivateUser
{
    public sealed record DeactivateUserCommand(Guid Id)
        : IRequest<DeactivateUserResponse>;
}

using MediatR;
using SIGTI.Domain.Enums;

namespace SIGTI.Application.Features.Users.Commands.CreateUser
{
    public sealed record CreateUserCommand(
        string Name,
        string Email,
        string Password,
        Role Role,
        Guid DepartmentId
    ) : IRequest<CreateUserResponse>;
}

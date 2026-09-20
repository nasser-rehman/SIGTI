using MediatR;
using SIGTI.Domain.Enums;

namespace SIGTI.Application.Features.Users.Commands.UpdateUser
{
    public sealed record UpdateUserCommand(
        Guid Id,
        string Name,
        Role Role,
        Guid DepartmentId
    ) : IRequest<UpdateUserResponse>;
}

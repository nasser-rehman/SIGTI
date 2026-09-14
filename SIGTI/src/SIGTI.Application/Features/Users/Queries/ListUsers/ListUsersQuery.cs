using MediatR;
using SIGTI.Domain.Enums;

namespace SIGTI.Application.Features.Users.Queries.ListUsers
{
    public sealed record ListUsersQuery(Role? Role = null)
        : IRequest<IReadOnlyCollection<ListUsersResponse>>;
}

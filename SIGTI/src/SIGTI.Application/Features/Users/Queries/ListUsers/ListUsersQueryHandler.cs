using MediatR;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Domain.Entities;

namespace SIGTI.Application.Features.Users.Queries.ListUsers
{
    public sealed class ListUsersQueryHandler
        : IRequestHandler<
            ListUsersQuery,
            IReadOnlyCollection<ListUsersResponse>
        >
    {
        private readonly IUserRepository _userRepository;

        public ListUsersQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IReadOnlyCollection<ListUsersResponse>> Handle(
            ListUsersQuery request,
            CancellationToken cancellationToken
        )
        {
            IReadOnlyCollection<User> users = request.Role.HasValue
                ? await _userRepository.ListByRoleAsync(
                    request.Role.Value,
                    cancellationToken
                )
                : await _userRepository.ListAllAsync(cancellationToken);

            return users
                .Select(user => new ListUsersResponse(
                    user.Id,
                    user.Name,
                    user.Email.Value,
                    user.Role,
                    user.DepartmentId,
                    user.IsActive
                ))
                .ToList();
        }
    }
}

using MediatR;
using SIGTI.Application.Common.Exceptions;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Domain.Entities;

namespace SIGTI.Application.Features.Users.Queries.GetUserById
{
    public sealed class GetUserByIdQueryHandler
        : IRequestHandler<GetUserByIdQuery, GetUserByIdResponse>
    {
        private readonly IUserRepository _userRepository;

        public GetUserByIdQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<GetUserByIdResponse> Handle(
            GetUserByIdQuery request,
            CancellationToken cancellationToken
        )
        {
            var user = await _userRepository.GetByIdAsync(
                request.Id,
                cancellationToken
            );

            if (user is null)
                throw new NotFoundException(nameof(User), request.Id);

            return new GetUserByIdResponse(
                user.Id,
                user.Name,
                user.Email.Value,
                user.Role,
                user.DepartmentId,
                user.IsActive,
                user.CreatedAt,
                user.UpdatedAt
            );
        }
    }
}

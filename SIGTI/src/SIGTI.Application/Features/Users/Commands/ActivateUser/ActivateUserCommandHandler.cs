using MediatR;
using SIGTI.Application.Common.Exceptions;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Domain.Entities;

namespace SIGTI.Application.Features.Users.Commands.ActivateUser
{
    public sealed class ActivateUserCommandHandler
        : IRequestHandler<ActivateUserCommand, ActivateUserResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ActivateUserCommandHandler(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork
        )
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ActivateUserResponse> Handle(
            ActivateUserCommand request,
            CancellationToken cancellationToken
        )
        {
            var user = await _userRepository.GetByIdAsync(
                request.Id,
                cancellationToken
            );

            if (user is null)
            {
                throw new NotFoundException(nameof(User), request.Id);
            }

            user.Activate();
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new ActivateUserResponse(
                user.Id,
                user.IsActive,
                user.UpdatedAt
            );
        }
    }
}

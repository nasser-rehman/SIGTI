using MediatR;
using SIGTI.Application.Common.Exceptions;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Application.Common.Interfaces.Services;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Exceptions;

namespace SIGTI.Application.Features.Users.Commands.DeactivateUser
{
    public sealed class DeactivateUserCommandHandler
        : IRequestHandler<DeactivateUserCommand, DeactivateUserResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public DeactivateUserCommandHandler(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService
        )
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<DeactivateUserResponse> Handle(
            DeactivateUserCommand request,
            CancellationToken cancellationToken
        )
        {
            var user = await _userRepository.GetByIdAsync(
                request.Id,
                cancellationToken
            );

            if (user is null)
                throw new NotFoundException(nameof(User), request.Id);

            if (user.IsSystem)
                throw new DomainException(
                    "Não é possível desativar um usuário do sistema."
                );

            if (_currentUserService.UserId == user.Id)
                throw new DomainException(
                    "O administrador não pode desativar o próprio usuário."
                );

            user.Deactivate();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new DeactivateUserResponse(
                user.Id,
                user.IsActive,
                user.UpdatedAt
            );
        }
    }
}

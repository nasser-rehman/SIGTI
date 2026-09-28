using MediatR;
using SIGTI.Application.Common.Exceptions;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Application.Common.Interfaces.Services;
using SIGTI.Domain.Entities;

namespace SIGTI.Application.Features.Users.Commands.ResetUserPassword
{
    public sealed class ResetUserPasswordCommandHandler
        : IRequestHandler<ResetUserPasswordCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUnitOfWork _unitOfWork;

        public ResetUserPasswordCommandHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IUnitOfWork unitOfWork
        )
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(
            ResetUserPasswordCommand request,
            CancellationToken cancellationToken
        )
        {
            var user = await _userRepository.GetByIdAsync(
                request.UserId,
                cancellationToken
            );

            if (user is null)
                throw new NotFoundException(nameof(User), request.UserId);

            var newPasswordHash = _passwordHasher.Hash(request.NewPassword);
            user.UpdatePasswordHash(newPasswordHash);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}

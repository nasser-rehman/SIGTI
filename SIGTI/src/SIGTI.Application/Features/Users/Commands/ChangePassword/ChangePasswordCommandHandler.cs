using MediatR;
using SIGTI.Application.Common.Exceptions;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Application.Common.Interfaces.Services;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Exceptions;

namespace SIGTI.Application.Features.Users.Commands.ChangePassword
{
    public sealed class ChangePasswordCommandHandler
        : IRequestHandler<ChangePasswordCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUnitOfWork _unitOfWork;

        public ChangePasswordCommandHandler(
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
            ChangePasswordCommand request,
            CancellationToken cancellationToken
        )
        {
            var user = await _userRepository.GetByIdAsync(
                request.UserId,
                cancellationToken
            );

            if (user is null)
                throw new NotFoundException(nameof(User), request.UserId);

            var isCurrentPasswordValid = _passwordHasher.Verify(
                request.CurrentPassword,
                user.PasswordHash
            );

            if (!isCurrentPasswordValid)
                throw new DomainException(
                    "A senha atual informada está incorreta."
                );

            var NewPasswordHash = _passwordHasher.Hash(request.NewPassword);
            user.UpdatePasswordHash(NewPasswordHash);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}

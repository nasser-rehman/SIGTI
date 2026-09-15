using MediatR;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Application.Common.Interfaces.Services;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Exceptions;
using SIGTI.Domain.ValueObjects;

namespace SIGTI.Application.Features.Users.Commands.CreateUser
{
    public sealed class CreateUserCommandHandler
        : IRequestHandler<CreateUserCommand, CreateUserResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEntityReferenceService _entityReferenceService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUnitOfWork _unitOfWork;

        public CreateUserCommandHandler(
            IUserRepository userRepository,
            IEntityReferenceService entityReferenceService,
            IPasswordHasher passwordHasher,
            IUnitOfWork unitOfWork
        )
        {
            _userRepository = userRepository;
            _entityReferenceService = entityReferenceService;
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateUserResponse> Handle(
            CreateUserCommand request,
            CancellationToken cancellationToken
        )
        {
            var emailVo = new Email(request.Email);

            if (
                await _userRepository.ExistsByEmailASync(
                    emailVo,
                    cancellationToken
                )
            )
            {
                throw new DomainException(
                    $"Já existe um usuário cadastrado com o e-mail'{request.Email}'."
                );
            }

            var department =
                await _entityReferenceService.GetRequiredDepartmentAsync(
                    request.DepartmentId,
                    cancellationToken
                );

            var passwordHash = _passwordHasher.Hash(request.Password);

            var user = new User(
                request.Name,
                emailVo,
                passwordHash,
                request.Role,
                department
            );

            await _userRepository.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CreateUserResponse(
                user.Id,
                user.Name,
                user.Email.Value,
                user.Role,
                user.DepartmentId,
                user.IsActive
            );
        }
    }
}

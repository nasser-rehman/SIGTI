using MediatR;
using SIGTI.Application.Common.Exceptions;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Application.Common.Interfaces.Services;
using SIGTI.Domain.Entities;

namespace SIGTI.Application.Features.Users.Commands.UpdateUser
{
    public sealed class UpdateUserCommandHandler
        : IRequestHandler<UpdateUserCommand, UpdateUserResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEntityReferenceService _entityReferenceService;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateUserCommandHandler(
            IUserRepository userRepository,
            IEntityReferenceService entityReferenceService,
            IUnitOfWork unitOfWork
        )
        {
            _userRepository = userRepository;
            _entityReferenceService = entityReferenceService;
            _unitOfWork = unitOfWork;
        }

        public async Task<UpdateUserResponse> Handle(
            UpdateUserCommand request,
            CancellationToken cancellationToken
        )
        {
            var user = await _userRepository.GetByIdAsync(
                request.Id,
                cancellationToken
            );

            if (user is null)
                throw new NotFoundException(nameof(User), request.Id);

            var department =
                await _entityReferenceService.GetRequiredDepartmentAsync(
                    request.DepartmentId,
                    cancellationToken
                );

            user.UpdateName(request.Name);
            user.UpdateRole(request.Role);
            user.ChangeDepartment(department);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new UpdateUserResponse(
                user.Id,
                user.Name,
                user.Email.Value,
                user.Role,
                user.DepartmentId,
                user.IsActive,
                user.UpdatedAt
            );
        }
    }
}

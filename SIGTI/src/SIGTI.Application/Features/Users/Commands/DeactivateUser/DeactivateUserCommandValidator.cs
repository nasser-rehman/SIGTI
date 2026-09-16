using FluentValidation;

namespace SIGTI.Application.Features.Users.Commands.DeactivateUser
{
    public sealed class DeactivateUserCommandValidator
        : AbstractValidator<DeactivateUserCommand>
    {
        public DeactivateUserCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("O ID do usuário é obrigatório.");
        }
    }
}

using FluentValidation;

namespace SIGTI.Application.Features.Users.Commands.ActivateUser
{
    public sealed class ActivateUserCommandValidator
        : AbstractValidator<ActivateUserCommand>
    {
        public ActivateUserCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("O identificador do usuário é obrigatório.");
        }
    }
}

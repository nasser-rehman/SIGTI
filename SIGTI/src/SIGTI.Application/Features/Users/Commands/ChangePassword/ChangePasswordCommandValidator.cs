using FluentValidation;

namespace SIGTI.Application.Features.Users.Commands.ChangePassword
{
    public sealed class ChangePasswordCommandValidator
        : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("O identificador do usuário é obrigatória.");

            RuleFor(x => x.CurrentPassword)
                .NotEmpty()
                .WithMessage("A senha atual é obrigatória.");

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .WithMessage("A nova senha é obrigatória.")
                .MinimumLength(6)
                .WithMessage("A nova senha deve ter no mínimo 6 caracteres.")
                .NotEqual(x => x.CurrentPassword)
                .WithMessage("A nova senha deve ser diferente da senha atual.");
        }
    }
}

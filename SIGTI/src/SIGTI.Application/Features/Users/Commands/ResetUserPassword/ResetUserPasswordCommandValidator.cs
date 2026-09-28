using FluentValidation;

namespace SIGTI.Application.Features.Users.Commands.ResetUserPassword
{
    public sealed class ResetUserPasswordCommandValidator
        : AbstractValidator<ResetUserPasswordCommand>
    {
        public ResetUserPasswordCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("O identificador do usuário é obrigatório.");

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .WithMessage("A nova senha é obrigatória.")
                .MinimumLength(6)
                .WithMessage("A nova senha deve ter no mínimo 6 caracteres.");
        }
    }
}

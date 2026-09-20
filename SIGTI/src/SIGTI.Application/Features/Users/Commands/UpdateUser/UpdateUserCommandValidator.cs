using FluentValidation;

namespace SIGTI.Application.Features.Users.Commands.UpdateUser
{
    public sealed class UpdateUserCommandValidator
        : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("O identificador do usuário é obrigatório.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("O nome não pode ser vazio.")
                .MinimumLength(10)
                .WithMessage("Tamanho mínimo de nome é 10 caracteres.")
                .MaximumLength(100)
                .WithMessage("Tamanho máximo de nome é 100 caracteres.");

            RuleFor(x => x.Role)
                .IsInEnum()
                .WithMessage("O cargo deve ser um valor válido.");

            RuleFor(x => x.DepartmentId)
                .NotEmpty()
                .WithMessage("O identificador do departamento é obrigatório.");
        }
    }
}

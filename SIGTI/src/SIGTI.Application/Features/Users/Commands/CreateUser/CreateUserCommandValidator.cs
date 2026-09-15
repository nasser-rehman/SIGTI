using FluentValidation;

namespace SIGTI.Application.Features.Users.Commands.CreateUser
{
    public sealed class CreateUserCommandValidator
        : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("O nome é obrigatório.")
                .Length(10, 100)
                .WithMessage("O nome deve ter entre 10 e 100 caracteres.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("O e-mail é obrigatório.")
                .EmailAddress()
                .WithMessage("O e-mail informado é inválido.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("A senha é obrigatória.")
                .MinimumLength(6)
                .WithMessage("A senha deve ter no mínimo 6 caracteres.");

            RuleFor(x => x.Role)
                .IsInEnum()
                .WithMessage("O papel (Role) informado é inválido.");

            RuleFor(x => x.DepartmentId)
                .NotEmpty()
                .WithMessage("O departamento é obrigatório.");
        }
    }
}

using FluentValidation;
using FluentValidation.Validators;

namespace SIGTI.Application.Features.Departments.Commands.CreateDepartment
{
    public sealed class CreateDepartmentCommandValidator
        : AbstractValidator<CreateDepartmentCommand>
    {
        public CreateDepartmentCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Nome do departamento é obrigatório.")
                .MaximumLength(150)
                .WithMessage(
                    "Tamanho máximo do departamento é 150 caracteres."
                );

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Descrição do departamento é obrigatório.")
                .MaximumLength(500)
                .WithMessage(
                    "Tamanho máximo da descrição do departamento é 500 caracteres."
                );
        }
    }
}

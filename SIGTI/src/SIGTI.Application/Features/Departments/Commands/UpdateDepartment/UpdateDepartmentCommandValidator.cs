using FluentValidation;

namespace SIGTI.Application.Features.Departments.Commands.UpdateDepartment
{
    public sealed class UpdateDepartmentCommandValidator
        : AbstractValidator<UpdateDepartmentCommand>
    {
        public UpdateDepartmentCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("O identificador do departamento é obrigatório.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("O nome do departamento é obrigatório.")
                .MaximumLength(150)
                .WithMessage("O tamanho máximo do nome é 150 caracteres.");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("A descrição do departamento é obrigatória.")
                .MaximumLength(500)
                .WithMessage("O tamanho máximo da descrição é 500 caracteres");
        }
    }
}

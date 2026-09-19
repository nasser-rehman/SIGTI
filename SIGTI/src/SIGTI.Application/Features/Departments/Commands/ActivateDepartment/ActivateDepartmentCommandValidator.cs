using FluentValidation;

namespace SIGTI.Application.Features.Departments.Commands.ActivateDepartment
{
    public sealed class ActivateDepartmentCommandValidator
        : AbstractValidator<ActivateDepartmentCommand>
    {
        public ActivateDepartmentCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("O identificador do departamento é obrigatório.");
        }
    }
}

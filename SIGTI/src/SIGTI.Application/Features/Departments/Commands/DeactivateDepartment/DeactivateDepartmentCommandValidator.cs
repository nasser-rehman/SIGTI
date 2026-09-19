using FluentValidation;

namespace SIGTI.Application.Features.Departments.Commands.DeactivateDepartment
{
    public sealed class DeactivateDepartmentCommandValidator
        : AbstractValidator<DeactivateDepartmentCommand>
    {
        public DeactivateDepartmentCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("O identificador do departamento é obrigatório.");
        }
    }
}

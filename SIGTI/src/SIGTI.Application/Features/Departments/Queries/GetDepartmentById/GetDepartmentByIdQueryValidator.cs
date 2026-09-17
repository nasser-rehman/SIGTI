using FluentValidation;

namespace SIGTI.Application.Features.Departments.Queries.GetDepartmentById
{
    public sealed class GetDepartmentByIdQueryValidator
        : AbstractValidator<GetDepartmentByIdQuery>
    {
        public GetDepartmentByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("O identificador do departamento é obrigatório.");
        }
    }
}

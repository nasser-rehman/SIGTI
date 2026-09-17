using FluentAssertions;
using FluentValidation.TestHelper;
using SIGTI.Application.Features.Departments.Queries.GetDepartmentById;

namespace SIGTI.Application.Tests.Features.Departments.Queries.GetDepartmentById
{
    public class GetDepartmentByIdQueryValidatorTests
    {
        private readonly GetDepartmentByIdQueryValidator _validator;

        public GetDepartmentByIdQueryValidatorTests()
        {
            _validator = new GetDepartmentByIdQueryValidator();
        }

        [Fact]
        public void Validate_WhenIdIsValid_ShouldNotHaveValidationErrors()
        {
            var query = new GetDepartmentByIdQuery(Guid.NewGuid());
            var result = _validator.TestValidate(query);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_WhenIdIsInvalid_ShouldHaveValidationErrors()
        {
            var query = new GetDepartmentByIdQuery(Guid.Empty);
            var result = _validator.TestValidate(query);
            result
                .ShouldHaveValidationErrorFor(x => x.Id)
                .WithErrorMessage(
                    "O identificador do departamento é obrigatório."
                );
        }
    }
}

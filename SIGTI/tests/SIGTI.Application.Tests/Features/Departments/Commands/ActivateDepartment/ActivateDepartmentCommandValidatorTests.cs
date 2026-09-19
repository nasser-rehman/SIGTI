using FluentValidation.TestHelper;
using SIGTI.Application.Features.Departments.Commands.ActivateDepartment;

namespace SIGTI.Application.Tests.Features.Departments.Commands.ActivateDepartment
{
    public class ActivateDepartmentCommandValidatorTests
    {
        private readonly ActivateDepartmentCommandValidator _validator;

        public ActivateDepartmentCommandValidatorTests()
        {
            _validator = new ActivateDepartmentCommandValidator();
        }

        [Fact]
        public void Validate_WhenDepartmentIdIsValid_ShouldNotHaveValidationErrors()
        {
            var command = new ActivateDepartmentCommand(Guid.NewGuid());
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_WhenDepartmentIdIsNullOrEmpty_ShouldHaveValidationErrors()
        {
            var command = new ActivateDepartmentCommand(Guid.Empty);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }
    }
}

using FluentValidation.TestHelper;
using SIGTI.Application.Features.Departments.Commands.DeactivateDepartment;

namespace SIGTI.Application.Tests.Features.Departments.Commands.DeactivateDepartment
{
    public class DeactivateDepartmentCommandValidatorTests
    {
        private readonly DeactivateDepartmentCommandValidator _validator;

        public DeactivateDepartmentCommandValidatorTests()
        {
            _validator = new DeactivateDepartmentCommandValidator();
        }

        [Fact]
        public void Validate_WhenDepartmentIdIsValid_ShouldNotHaveValidationErrors()
        {
            var command = new DeactivateDepartmentCommand(Guid.NewGuid());
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_WhenDepartmentIdIsNullOrEmpty_ShouldHaveValidationErrors()
        {
            var command = new DeactivateDepartmentCommand(Guid.Empty);
            var result = _validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }
    }
}

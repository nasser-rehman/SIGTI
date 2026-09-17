using FluentValidation.TestHelper;
using SIGTI.Application.Features.Departments.Commands.UpdateDepartment;

namespace SIGTI.Application.Tests.Features.Departments.Commands.UpdateDepartment
{
    public class UpdateDepartmentCommandValidatorTests
    {
        private readonly UpdateDepartmentCommandValidator _validator;

        public UpdateDepartmentCommandValidatorTests()
        {
            _validator = new UpdateDepartmentCommandValidator();
        }

        [Fact]
        public void Validate_WhenCommandIsValid_ShouldNotHaveValidationErrors()
        {
            var command = new UpdateDepartmentCommand(
                Guid.NewGuid(),
                "Departamento de TI",
                "Descrição válida do departamento"
            );

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_WhenIdIsEmpty_ShouldHaveValidationError()
        {
            var command = new UpdateDepartmentCommand(
                Guid.Empty,
                "Departamento de TI",
                "Descrição válida"
            );

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Validate_WhenNameIsInvalid_ShouldHaveValidationError(
            string? invalidName
        )
        {
            var command = new UpdateDepartmentCommand(
                Guid.NewGuid(),
                invalidName!,
                "Descrição válida"
            );

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Validate_WhenNameExceedsMaxLength_ShouldHaveValidationError()
        {
            var command = new UpdateDepartmentCommand(
                Guid.NewGuid(),
                new string('A', 151),
                "Descrição válida"
            );

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Name);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Validate_WhenDescriptionIsInvalid_ShouldHaveValidationError(
            string? invalidDescription
        )
        {
            var command = new UpdateDepartmentCommand(
                Guid.NewGuid(),
                "Nome Válido",
                invalidDescription!
            );

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Validate_WhenDescriptionExceedsMaxLength_ShouldHaveValidationError()
        {
            var command = new UpdateDepartmentCommand(
                Guid.NewGuid(),
                "Nome Válido",
                new string('A', 501)
            );

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Description);
        }
    }
}

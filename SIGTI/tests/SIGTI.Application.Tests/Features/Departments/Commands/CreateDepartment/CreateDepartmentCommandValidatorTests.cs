using FluentAssertions;
using SIGTI.Application.Features.Departments.Commands.CreateDepartment;

namespace SIGTI.Application.Tests.Features.Departments.Commands.CreateDepartment
{
    public class CreateDepartmentCommandValidatorTests
    {
        private readonly CreateDepartmentCommandValidator _validator = new();

        [Fact]
        public void Validate_WhenCommandIsValid_ShouldNotHaveValidationError()
        {
            var command = new CreateDepartmentCommand(
                "Test name 123",
                "Description test"
            );

            var result = _validator.Validate(command);

            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Validate_WhenNameIsEmpty_ShouldHaveValidationError(
            string? name
        )
        {
            var command = new CreateDepartmentCommand(name, "Test Empty title");

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result
                .Errors.Should()
                .Contain(error =>
                    error.PropertyName == nameof(CreateDepartmentCommand.Name)
                );
        }

        [Fact]
        public void Validate_WhenNameExceedsMaxLength_ShouldHaveValidationError()
        {
            var maxLengthString = new string('A', 151);
            var command = new CreateDepartmentCommand(
                maxLengthString,
                "Test Max Length Name"
            );

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result
                .Errors.Should()
                .Contain(error =>
                    error.PropertyName == nameof(CreateDepartmentCommand.Name)
                );
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Validate_WhenDescriptionIsEmpty_ShouldHaveValidationError(
            string? description
        )
        {
            var command = new CreateDepartmentCommand(
                "Title with empty description",
                description
            );

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result
                .Errors.Should()
                .Contain(error =>
                    error.PropertyName
                    == nameof(CreateDepartmentCommand.Description)
                );
        }

        [Fact]
        public void Validate_WhenDescriptionExceedsMaxLength_ShouldHaveValidationError()
        {
            var maxLengthDescription = new string('a', 501);
            var command = new CreateDepartmentCommand(
                "Title with Description Max Length Exceed",
                maxLengthDescription
            );

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result
                .Errors.Should()
                .Contain(error =>
                    error.PropertyName
                    == nameof(CreateDepartmentCommand.Description)
                );
        }
    }
}

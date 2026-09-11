using FluentAssertions;
using SIGTI.Application.Features.SupportQueues.Commands.CreateSupportQueue;

namespace SIGTI.Application.Tests.Features.SupportQueues.Commands.CreateSupportQueue
{
    public class CreateSupportQueueCommandValidatorTests
    {
        private readonly CreateSupportQueueCommandValidator _validator = new();

        [Fact]
        public void Validate_WhenCommandIsValid_ShouldNotHaveValidationError()
        {
            var command = new CreateSupportQueueCommand(
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
            var command = new CreateSupportQueueCommand(
                name,
                "Test Empty title"
            );

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result
                .Errors.Should()
                .Contain(error =>
                    error.PropertyName == nameof(CreateSupportQueueCommand.Name)
                );
        }

        [Fact]
        public void Validate_WhenNameExceedsMaxLength_ShouldHaveValidationError()
        {
            var maxLengthString = new string('A', 151);
            var command = new CreateSupportQueueCommand(
                maxLengthString,
                "Test Max Length Name"
            );

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result
                .Errors.Should()
                .Contain(error =>
                    error.PropertyName == nameof(CreateSupportQueueCommand.Name)
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
            var command = new CreateSupportQueueCommand(
                "Title with empty description",
                description
            );

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result
                .Errors.Should()
                .Contain(error =>
                    error.PropertyName
                    == nameof(CreateSupportQueueCommand.Description)
                );
        }

        [Fact]
        public void Validate_WhenDescriptionExceedsMaxLength_ShouldHaveValidationError()
        {
            var maxLengthDescription = new string('a', 501);
            var command = new CreateSupportQueueCommand(
                "Title with Description Max Length Exceed",
                maxLengthDescription
            );

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result
                .Errors.Should()
                .Contain(error =>
                    error.PropertyName
                    == nameof(CreateSupportQueueCommand.Description)
                );
        }
    }
}

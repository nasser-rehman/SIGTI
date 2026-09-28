using FluentAssertions;
using SIGTI.Application.Features.Users.Commands.ResetUserPassword;

namespace SIGTI.Application.Tests.Features.Users.Commands.ResetUserPassword
{
    public class ResetUserPasswordCommandValidatorTests
    {
        private readonly ResetUserPasswordCommandValidator _validator = new();

        [Fact]
        public void Validate_ValidCommand_ShouldNotHaveValidationErrors()
        {
            var command = new ResetUserPasswordCommand(
                Guid.NewGuid(),
                "NovaSenha123"
            );

            var result = _validator.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EmptyUserId_ShouldHaveValidationError()
        {
            var command = new ResetUserPasswordCommand(
                Guid.Empty,
                "NovaSenha123"
            );

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result
                .Errors.Should()
                .Contain(e =>
                    e.PropertyName == nameof(ResetUserPasswordCommand.UserId)
                );
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        [InlineData("12345")]
        public void Validate_InvalidNewPassword_ShouldHaveValidationError(
            string? invalidNewPassword
        )
        {
            var command = new ResetUserPasswordCommand(
                Guid.NewGuid(),
                invalidNewPassword!
            );

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result
                .Errors.Should()
                .Contain(e =>
                    e.PropertyName
                    == nameof(ResetUserPasswordCommand.NewPassword)
                );
        }
    }
}

using FluentAssertions;
using SIGTI.Application.Features.Users.Commands.ChangePassword;

namespace SIGTI.Application.Tests.Features.Users.Commands.ChangePassword
{
    public class ChangePasswordCommandValidatorTests
    {
        private readonly ChangePasswordCommandValidator _validator = new();

        [Fact]
        public void Validate_ValidCommand_ShouldNotHaveValidationErrors()
        {
            var command = new ChangePasswordCommand(
                Guid.NewGuid(),
                "SenhaAtual123",
                "NovaSenha123"
            );

            var result = _validator.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_EmptyUserId_ShouldHaveValidationError()
        {
            var command = new ChangePasswordCommand(
                Guid.Empty,
                "SenhaAtual123",
                "NovaSenha123"
            );

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result
                .Errors.Should()
                .Contain(e =>
                    e.PropertyName == nameof(ChangePasswordCommand.UserId)
                );
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Validate_InvalidCurrentPassword_ShouldHaveValidationError(
            string? invalidCurrentPassword
        )
        {
            var command = new ChangePasswordCommand(
                Guid.NewGuid(),
                invalidCurrentPassword!,
                "NovaSenha123"
            );

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result
                .Errors.Should()
                .Contain(e =>
                    e.PropertyName
                    == nameof(ChangePasswordCommand.CurrentPassword)
                );
        }

        [Theory]
        [InlineData("")]
        [InlineData("12345")]
        public void Validate_InvalidNewPasswordLength_ShouldHaveValidationError(
            string invalidNewPassword
        )
        {
            var command = new ChangePasswordCommand(
                Guid.NewGuid(),
                "SenhaAtual123",
                invalidNewPassword
            );

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result
                .Errors.Should()
                .Contain(e =>
                    e.PropertyName == nameof(ChangePasswordCommand.NewPassword)
                );
        }

        [Fact]
        public void Validate_NewPasswordEqualToCurrent_ShouldHaveValidationError()
        {
            var command = new ChangePasswordCommand(
                Guid.NewGuid(),
                "MesmaSenha123",
                "MesmaSenha123"
            );

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result
                .Errors.Should()
                .Contain(e =>
                    e.PropertyName == nameof(ChangePasswordCommand.NewPassword)
                    && e.ErrorMessage.Contains("diferente")
                );
        }
    }
}

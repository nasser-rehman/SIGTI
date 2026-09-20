using FluentAssertions;
using FluentValidation.TestHelper;
using SIGTI.Application.Features.Users.Commands.UpdateUser;
using SIGTI.Domain.Enums;

namespace SIGTI.Application.Tests.Features.Users.Commands.UpdateUser
{
    public class UpdateUserCommandValidatorTests
    {
        private readonly UpdateUserCommandValidator _validator;

        public UpdateUserCommandValidatorTests()
        {
            _validator = new UpdateUserCommandValidator();
        }

        [Fact]
        public void Validate_WhenIdIsEmpty_ShouldHaveValidationError()
        {
            var command = new UpdateUserCommand(
                Guid.Empty,
                "Cool name test",
                Role.User,
                Guid.NewGuid()
            );
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(x => x.Id)
                .WithErrorMessage("O identificador do usuário é obrigatório.");
        }

        [Fact]
        public void Validate_WhenNameIsEmpty_ShouldHaveValidationError()
        {
            var command = new UpdateUserCommand(
                Guid.NewGuid(),
                "",
                Role.User,
                Guid.NewGuid()
            );
            var result = _validator.TestValidate(command);

            result
                .ShouldHaveValidationErrorFor(x => x.Name)
                .WithErrorMessage("O nome não pode ser vazio.");
        }

        [Fact]
        public void Validate_WhenNameIsShorterThan10Chars_ShouldHaveValidationError()
        {
            var command = new UpdateUserCommand(
                Guid.NewGuid(),
                "Ahoi",
                Role.User,
                Guid.NewGuid()
            );
            var result = _validator.TestValidate(command);

            result
                .ShouldHaveValidationErrorFor(x => x.Name)
                .WithErrorMessage("Tamanho mínimo de nome é 10 caracteres.");
        }

        [Fact]
        public void Validate_WhenNameIsLongerThan100Chars_ShouldHaveValidationError()
        {
            var command = new UpdateUserCommand(
                Guid.NewGuid(),
                new string('a', 101),
                Role.User,
                Guid.NewGuid()
            );
            var result = _validator.TestValidate(command);

            result
                .ShouldHaveValidationErrorFor(x => x.Name)
                .WithErrorMessage("Tamanho máximo de nome é 100 caracteres.");
        }

        [Fact]
        public void Validate_WhenRoleIsInvalid_ShouldHaveValidationError()
        {
            var command = new UpdateUserCommand(
                Guid.NewGuid(),
                "Cool name test",
                (Role)999,
                Guid.NewGuid()
            );
            var result = _validator.TestValidate(command);

            result
                .ShouldHaveValidationErrorFor(x => x.Role)
                .WithErrorMessage("O cargo deve ser um valor válido.");
        }

        [Fact]
        public void Validate_WhenDepartmentIdIsEmpty_ShouldHaveValidationError()
        {
            var command = new UpdateUserCommand(
                Guid.NewGuid(),
                "Cool name test",
                Role.User,
                Guid.Empty
            );

            var result = _validator.TestValidate(command);

            result
                .ShouldHaveValidationErrorFor(x => x.DepartmentId)
                .WithErrorMessage(
                    "O identificador do departamento é obrigatório."
                );
        }

        [Fact]
        public void Validate_WhenCommandIsValid_ShouldNotHaveValidationErrors()
        {
            var command = new UpdateUserCommand(
                Guid.NewGuid(),
                "Cool name test",
                Role.User,
                Guid.NewGuid()
            );

            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}

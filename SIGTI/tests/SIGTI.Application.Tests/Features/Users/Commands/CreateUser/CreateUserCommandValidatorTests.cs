using FluentValidation.TestHelper;
using SIGTI.Application.Features.Users.Commands.CreateUser;
using SIGTI.Domain.Enums;

namespace SIGTI.Application.Tests.Features.Users.Commands.CreateUser
{
    public class CreateUserCommandValidatorTests
    {
        private readonly CreateUserCommandValidator _validator;

        public CreateUserCommandValidatorTests()
        {
            _validator = new CreateUserCommandValidator();
        }

        [Fact]
        public void Validate_WhenCommandIsValid_ShouldNotHaveValidationErrors()
        {
            var command = new CreateUserCommand(
                "Nasser Rehman",
                "nasser@sigti.local",
                "Senha@123",
                Role.Technician,
                Guid.NewGuid()
            );

            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Validate_WhenNameIsEmpty_ShouldHaveValidationError(
            string invalidName
        )
        {
            var command = new CreateUserCommand(
                invalidName,
                "nasser@sigti.local",
                "Senha@123",
                Role.Technician,
                Guid.NewGuid()
            );

            var result = _validator.TestValidate(command);

            result
                .ShouldHaveValidationErrorFor(x => x.Name)
                .WithErrorMessage("O nome é obrigatório.");
        }

        [Fact]
        public void Validate_WhenNameIsShorterThan10Chars_ShouldHaveValidationError()
        {
            var command = new CreateUserCommand(
                "Short",
                "nasser@sigti.local",
                "Senha@123",
                Role.Technician,
                Guid.NewGuid()
            );

            var result = _validator.TestValidate(command);

            result
                .ShouldHaveValidationErrorFor(x => x.Name)
                .WithErrorMessage("O nome deve ter entre 10 e 100 caracteres.");
        }

        [Fact]
        public void Validate_WhenNameIsLongerThan100Chars_ShouldHaveValidationError()
        {
            var command = new CreateUserCommand(
                new string('a', 101),
                "nasser@sigti.local",
                "Senha@123",
                Role.Technician,
                Guid.NewGuid()
            );

            var result = _validator.TestValidate(command);

            result
                .ShouldHaveValidationErrorFor(x => x.Name)
                .WithErrorMessage("O nome deve ter entre 10 e 100 caracteres.");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Validate_WhenEmailIsEmpty_ShouldHaveValidationError(
            string invalidEmail
        )
        {
            var command = new CreateUserCommand(
                "Nasser Rehman",
                invalidEmail,
                "Senha@123",
                Role.Technician,
                Guid.NewGuid()
            );

            var result = _validator.TestValidate(command);

            result
                .ShouldHaveValidationErrorFor(x => x.Email)
                .WithErrorMessage("O e-mail é obrigatório.");
        }

        [Fact]
        public void Validate_WhenEmailFormatIsInvalid_ShouldHaveValidationError()
        {
            var command = new CreateUserCommand(
                "Nasser Rehman",
                "invalid_email_without_arroba",
                "Senha@123",
                Role.Technician,
                Guid.NewGuid()
            );

            var result = _validator.TestValidate(command);

            result
                .ShouldHaveValidationErrorFor(x => x.Email)
                .WithErrorMessage("O e-mail informado é inválido.");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Validate_WhenPasswordIsEmpty_ShouldHaveValidationError(
            string invalidPassword
        )
        {
            var command = new CreateUserCommand(
                "Nasser Rehman",
                "nasser@sigti.local",
                invalidPassword,
                Role.Technician,
                Guid.NewGuid()
            );

            var result = _validator.TestValidate(command);

            result
                .ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorMessage("A senha é obrigatória.");
        }

        [Fact]
        public void Validate_WhenPasswordIsShorterThan6Chars_ShouldHaveValidationError()
        {
            var command = new CreateUserCommand(
                "Nasser Rehman",
                "nasser@sigti.local",
                "12345",
                Role.Technician,
                Guid.NewGuid()
            );

            var result = _validator.TestValidate(command);

            result
                .ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorMessage("A senha deve ter no mínimo 6 caracteres.");
        }

        [Fact]
        public void Validate_WhenRoleIsInvalidEnum_ShouldHaveValidationError()
        {
            var command = new CreateUserCommand(
                "Nasser Rehman",
                "nasser@sigti.local",
                "Senha@123",
                (Role)999,
                Guid.NewGuid()
            );

            var result = _validator.TestValidate(command);

            result
                .ShouldHaveValidationErrorFor(x => x.Role)
                .WithErrorMessage("O papel (Role) informado é inválido.");
        }

        [Fact]
        public void Validate_WhenDepartmentIdIsEmpty_ShouldHaveValidationError()
        {
            var command = new CreateUserCommand(
                "Nasser Rehman",
                "nasser@sigti.local",
                "Senha@123",
                Role.Technician,
                Guid.Empty
            );

            var result = _validator.TestValidate(command);

            result
                .ShouldHaveValidationErrorFor(x => x.DepartmentId)
                .WithErrorMessage("O departamento é obrigatório.");
        }
    }
}

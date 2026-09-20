using FluentValidation.TestHelper;
using SIGTI.Application.Features.Users.Commands.ActivateUser;
using SIGTI.Application.Features.Users.Commands.CreateUser;

namespace SIGTI.Application.Tests.Features.Users.Commands.ActivateUser
{
    public class ActivateUserCommandValidatorTests
    {
        private readonly ActivateUserCommandValidator _validator;

        public ActivateUserCommandValidatorTests()
        {
            _validator = new ActivateUserCommandValidator();
        }

        [Fact]
        public void Validate_WhenIdIsEmpty_ShouldHaveValidationError()
        {
            var command = new ActivateUserCommand(Guid.Empty);
            var response = _validator.TestValidate(command);
            response
                .ShouldHaveValidationErrorFor(x => x.Id)
                .WithErrorMessage("O identificador do usuário é obrigatório.");
        }

        [Fact]
        public void Validate_WhenIdIsValid_ShouldNotHaveValidationError()
        {
            var command = new ActivateUserCommand(Guid.NewGuid());
            var response = _validator.TestValidate(command);
            response.ShouldNotHaveAnyValidationErrors();
        }
    }
}

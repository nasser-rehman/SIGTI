using FluentValidation.TestHelper;
using SIGTI.Application.Features.Users.Commands.DeactivateUser;

namespace SIGTI.Application.Tests.Features.Users.Commands.DeactivateUser
{
    public class DeactivateUserCommandValidatorTests
    {
        private readonly DeactivateUserCommandValidator _validator;

        public DeactivateUserCommandValidatorTests()
        {
            _validator = new DeactivateUserCommandValidator();
        }

        [Fact]
        public void Validate_WhenIdIsValid_ShouldNotHaveValidationErrors()
        {
            var command = new DeactivateUserCommand(Guid.NewGuid());
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_WhenIdIsEmpty_ShouldHaveValidationError()
        {
            var command = new DeactivateUserCommand(Guid.Empty);
            var result = _validator.TestValidate(command);

            result
                .ShouldHaveValidationErrorFor(x => x.Id)
                .WithErrorMessage("O ID do usuário é obrigatório.");
        }
    }
}

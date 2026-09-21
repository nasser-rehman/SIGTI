using FluentValidation.TestHelper;
using SIGTI.Application.Features.SupportQueues.Commands.DeactivateSupportQueue;
using Xunit;

namespace SIGTI.Application.Tests.Features.SupportQueues.Commands.DeactivateSupportQueue
{
    public class DeactivateSupportQueueCommandValidatorTests
    {
        private readonly DeactivateSupportQueueCommandValidator _validator;

        public DeactivateSupportQueueCommandValidatorTests()
        {
            _validator = new DeactivateSupportQueueCommandValidator();
        }

        [Fact]
        public void Validate_WhenIdIsValid_ShouldNotHaveValidationErrors()
        {
            var command = new DeactivateSupportQueueCommand(Guid.NewGuid());
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_WhenIdIsEmpty_ShouldHaveValidationError()
        {
            var command = new DeactivateSupportQueueCommand(Guid.Empty);
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(x => x.Id)
                .WithErrorMessage(
                    "O identificador da fila de suporte é obrigatório."
                );
        }
    }
}

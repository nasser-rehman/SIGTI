using FluentValidation.TestHelper;
using SIGTI.Application.Features.SupportQueues.Commands.ActivateSupportQueue;
using Xunit;

namespace SIGTI.Application.Tests.Features.SupportQueues.Commands.ActivateSupportQueue
{
    public class ActivateSupportQueueCommandValidatorTests
    {
        private readonly ActivateSupportQueueCommandValidator _validator;

        public ActivateSupportQueueCommandValidatorTests()
        {
            _validator = new ActivateSupportQueueCommandValidator();
        }

        [Fact]
        public void Validate_WhenIdIsValid_ShouldNotHaveValidationErrors()
        {
            var command = new ActivateSupportQueueCommand(Guid.NewGuid());
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_WhenIdIsEmpty_ShouldHaveValidationError()
        {
            var command = new ActivateSupportQueueCommand(Guid.Empty);
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(x => x.Id)
                .WithErrorMessage(
                    "O identificador da fila de suporte é obrigatório."
                );
        }
    }
}

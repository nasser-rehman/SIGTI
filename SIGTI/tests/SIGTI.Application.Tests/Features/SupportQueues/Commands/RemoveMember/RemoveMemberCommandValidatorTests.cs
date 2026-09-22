using FluentValidation.TestHelper;
using SIGTI.Application.Features.SupportQueues.Commands.RemoveMember;

namespace SIGTI.Application.Tests.Features.SupportQueues.Commands.RemoveMember
{
    public class RemoveMemberCommandValidatorTests
    {
        private readonly RemoveMemberCommandValidator _validator;

        public RemoveMemberCommandValidatorTests()
        {
            _validator = new RemoveMemberCommandValidator();
        }

        [Fact]
        public void Validate_WhenCommandIsValid_ShouldNotHaveValidationErrors()
        {
            var command = new RemoveMemberCommand(
                Guid.NewGuid(),
                Guid.NewGuid()
            );
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_WhenQueueIdIsEmpty_ShouldHaveValidationError()
        {
            var command = new RemoveMemberCommand(Guid.Empty, Guid.NewGuid());
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(x => x.QueueId)
                .WithErrorMessage(
                    "O identificador da fila de suporte é obrigatório."
                );
        }

        [Fact]
        public void Validate_WhenTechnicianIdIsEmpty_ShouldHaveValidationError()
        {
            var command = new RemoveMemberCommand(Guid.NewGuid(), Guid.Empty);
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(x => x.TechnicianId)
                .WithErrorMessage("O identificador do técnico é obrigatório.");
        }
    }
}

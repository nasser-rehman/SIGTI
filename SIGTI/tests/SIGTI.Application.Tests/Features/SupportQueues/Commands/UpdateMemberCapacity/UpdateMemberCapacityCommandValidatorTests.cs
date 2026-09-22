using FluentAssertions;
using FluentValidation.TestHelper;
using SIGTI.Application.Features.SupportQueues.Commands.UpdateMemberCapacity;

namespace SIGTI.Application.Tests.Features.SupportQueues.Commands.UpdateMemberCapacity
{
    public class UpdateMemberCapacityCommandValidatorTests
    {
        private readonly UpdateMemberCapacityCommandValidator _validator;

        public UpdateMemberCapacityCommandValidatorTests()
        {
            _validator = new UpdateMemberCapacityCommandValidator();
        }

        [Fact]
        public void Validate_WhenCommandIsValid_ShouldNotHaveValidationErrors()
        {
            var command = new UpdateMemberCapacityCommand(
                Guid.NewGuid(),
                Guid.NewGuid(),
                5
            );

            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_WhenQueueIdIsEmpty_ShouldHaveValidationError()
        {
            var command = new UpdateMemberCapacityCommand(
                Guid.Empty,
                Guid.NewGuid(),
                5
            );

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
            var command = new UpdateMemberCapacityCommand(
                Guid.NewGuid(),
                Guid.Empty,
                5
            );

            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(x => x.TechnicianId)
                .WithErrorMessage("O identificador do técnico é obrigatório.");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Validate_WhenMaxConcurrentTicketsIsZeroOrNegative_ShouldHaveValidationError(
            int invalidCapacity
        )
        {
            var command = new UpdateMemberCapacityCommand(
                Guid.NewGuid(),
                Guid.NewGuid(),
                invalidCapacity
            );

            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(x => x.MaxConcurrentTickets)
                .WithErrorMessage(
                    "O limite de chamados simultâneos deve ser maior que zero."
                );
        }
    }
}

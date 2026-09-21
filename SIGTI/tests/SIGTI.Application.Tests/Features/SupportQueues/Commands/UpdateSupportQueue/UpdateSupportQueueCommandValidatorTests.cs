using FluentValidation.TestHelper;
using SIGTI.Application.Features.SupportQueues.Commands.UpdateSupportQueue;
using Xunit;

namespace SIGTI.Application.Tests.Features.SupportQueues.Commands.UpdateSupportQueue
{
    public class UpdateSupportQueueCommandValidatorTests
    {
        private readonly UpdateSupportQueueCommandValidator _validator;

        public UpdateSupportQueueCommandValidatorTests()
        {
            _validator = new UpdateSupportQueueCommandValidator();
        }

        [Fact]
        public void Validate_WhenCommandIsValid_ShouldNotHaveValidationErrors()
        {
            var command = new UpdateSupportQueueCommand(
                Guid.NewGuid(),
                "Fila Atualizada",
                "Descrição válida da fila."
            );

            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_WhenIdIsEmpty_ShouldHaveValidationError()
        {
            var command = new UpdateSupportQueueCommand(
                Guid.Empty,
                "Fila Atualizada",
                "Descrição válida."
            );

            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(x => x.Id)
                .WithErrorMessage(
                    "O identificador da fila de suporte é obrigatório."
                );
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Validate_WhenNameIsEmpty_ShouldHaveValidationError(
            string? invalidName
        )
        {
            var command = new UpdateSupportQueueCommand(
                Guid.NewGuid(),
                invalidName!,
                "Descrição válida."
            );

            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(x => x.Name)
                .WithErrorMessage("O nome da fila de suporte é obrigatório.");
        }

        [Fact]
        public void Validate_WhenNameExceedsMaxLength_ShouldHaveValidationError()
        {
            var command = new UpdateSupportQueueCommand(
                Guid.NewGuid(),
                new string('A', 151),
                "Descrição válida."
            );

            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(x => x.Name)
                .WithErrorMessage(
                    "O nome da fila de suporte deve ter no máximo 150 caracteres."
                );
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Validate_WhenDescriptionIsEmpty_ShouldHaveValidationError(
            string? invalidDescription
        )
        {
            var command = new UpdateSupportQueueCommand(
                Guid.NewGuid(),
                "Nome Válido",
                invalidDescription!
            );

            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(x => x.Description)
                .WithErrorMessage(
                    "A descrição da fila de suporte é obrigatória."
                );
        }

        [Fact]
        public void Validate_WhenDescriptionExceedsMaxLength_ShouldHaveValidationError()
        {
            var command = new UpdateSupportQueueCommand(
                Guid.NewGuid(),
                "Nome Válido",
                new string('A', 501)
            );

            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(x => x.Description)
                .WithErrorMessage(
                    "A descrição da fila de suporte deve ter no máximo 500 caracteres."
                );
        }
    }
}

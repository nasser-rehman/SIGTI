using FluentValidation.TestHelper;
using SIGTI.Application.Features.Tickets.Commands.ReclassifyTicket;
using SIGTI.Domain.Enums;
using Xunit;

namespace SIGTI.Application.Tests.Features.Tickets.Commands.ReclassifyTicket
{
    public class ReclassifyTicketCommandValidatorTests
    {
        private readonly ReclassifyTicketCommandValidator _validator;

        public ReclassifyTicketCommandValidatorTests()
        {
            _validator = new ReclassifyTicketCommandValidator();
        }

        [Fact]
        public void Validate_WhenCommandIsValid_ShouldNotHaveValidationErrors()
        {
            var command = new ReclassifyTicketCommand(
                Guid.NewGuid(),
                TicketPriority.High,
                TicketCategory.Hardware
            );

            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_WhenTicketIdIsEmpty_ShouldHaveValidationError()
        {
            var command = new ReclassifyTicketCommand(
                Guid.Empty,
                TicketPriority.High,
                TicketCategory.Hardware
            );

            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(x => x.TicketId)
                .WithErrorMessage("O identificador do ticket é obrigatório.");
        }

        [Fact]
        public void Validate_WhenPriorityIsInvalid_ShouldHaveValidationError()
        {
            var command = new ReclassifyTicketCommand(
                Guid.NewGuid(),
                (TicketPriority)999,
                TicketCategory.Hardware
            );

            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(x => x.Priority)
                .WithErrorMessage("Prioridade do ticket inválida.");
        }

        [Fact]
        public void Validate_WhenCategoryIsInvalid_ShouldHaveValidationError()
        {
            var command = new ReclassifyTicketCommand(
                Guid.NewGuid(),
                TicketPriority.Medium,
                (TicketCategory)999
            );

            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(x => x.Category)
                .WithErrorMessage("Categoria do ticket inválida.");
        }
    }
}

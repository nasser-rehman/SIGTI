using FluentValidation.TestHelper;
using SIGTI.Application.Features.Tickets.Commands.ResumeTicketService;
using Xunit;

namespace SIGTI.Application.Tests.Features.Tickets.Commands.ResumeTicketService
{
    public class ResumeTicketServiceValidatorTests
    {
        private readonly ResumeTicketServiceCommandValidator _validator;

        public ResumeTicketServiceValidatorTests()
        {
            _validator = new ResumeTicketServiceCommandValidator();
        }

        [Fact]
        public void Validate_WhenCommandIsValid_ShouldNotHaveValidationErrors()
        {
            var command = new ResumeTicketServiceCommand(Guid.NewGuid());
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_WhenTicketIdIsEmpty_ShouldHaveValidationError()
        {
            var command = new ResumeTicketServiceCommand(Guid.Empty);
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(x => x.TicketId)
                .WithErrorMessage("O identificador do ticket é obrigatório.");
        }
    }
}

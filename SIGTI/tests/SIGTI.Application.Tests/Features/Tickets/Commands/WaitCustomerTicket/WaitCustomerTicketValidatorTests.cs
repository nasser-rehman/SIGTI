using FluentValidation.TestHelper;
using SIGTI.Application.Features.Tickets.Commands.WaitCustomerTicket;

namespace SIGTI.Application.Tests.Features.Tickets.Commands.WaitCustomerTicket
{
    public class WaitCustomerTicketValidatorTests
    {
        private readonly WaitCustomerTicketValidator _validator;

        public WaitCustomerTicketValidatorTests()
        {
            _validator = new WaitCustomerTicketValidator();
        }

        [Fact]
        public void Validate_WhenCommandIsValid_ShouldNotHaveValidationErrors()
        {
            var command = new WaitCustomerTicketCommand(Guid.NewGuid());
            var result = _validator.TestValidate(command);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_WhenTicketIdIsEmpty_ShouldHaveValidationError()
        {
            var command = new WaitCustomerTicketCommand(Guid.Empty);
            var result = _validator.TestValidate(command);
            result
                .ShouldHaveValidationErrorFor(x => x.TicketId)
                .WithErrorMessage("O identificador do ticket é obrigatório.");
        }
    }
}

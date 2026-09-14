using FluentValidation.TestHelper;
using SIGTI.Application.Features.Tickets.Queries.GetTicketTimeline;

namespace SIGTI.Application.Tests.Features.Tickets.Queries.GetTicketTimeline
{
    public class GetTicketTimelineQueryValidatorTests
    {
        private readonly GetTicketTimelineQueryValidator _validator;

        public GetTicketTimelineQueryValidatorTests()
        {
            _validator = new GetTicketTimelineQueryValidator();
        }

        [Fact]
        public void Validate_WhenTicketIdIsEmpty_ShouldHaveValidationError()
        {
            var query = new GetTicketTimelineQuery(Guid.Empty);

            var result = _validator.TestValidate(query);

            result
                .ShouldHaveValidationErrorFor(x => x.TicketId)
                .WithErrorMessage("O identificador do ticket é obrigatório.");
        }

        [Fact]
        public void Validate_WHenTicketIdIsValid_ShouldNotHaveValidationError()
        {
            var query = new GetTicketTimelineQuery(Guid.NewGuid());

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}

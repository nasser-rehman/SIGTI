using FluentValidation.TestHelper;
using SIGTI.Application.Features.SupportQueues.Queries.GetSupportQueueById;

namespace SIGTI.Application.Tests.Features.SupportQueues.Queries.GetSupportQueueById
{
    public class GetSupportQueueByIdQueryValidatorTests
    {
        private readonly GetSupportQueueByIdQueryValidator _validator;

        public GetSupportQueueByIdQueryValidatorTests()
        {
            _validator = new GetSupportQueueByIdQueryValidator();
        }

        [Fact]
        public void Validate_WhenIdIsValid_ShouldNotHaveValidationErrors()
        {
            var query = new GetSupportQueueByIdQuery(Guid.NewGuid());
            var result = _validator.TestValidate(query);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_WhenIdIsInvalid_ShouldHaveValidationErrors()
        {
            var query = new GetSupportQueueByIdQuery(Guid.Empty);
            var result = _validator.TestValidate(query);
            result
                .ShouldHaveValidationErrorFor(x => x.Id)
                .WithErrorMessage(
                    "O identificador da fila de suporte é obrigatório."
                );
        }
    }
}

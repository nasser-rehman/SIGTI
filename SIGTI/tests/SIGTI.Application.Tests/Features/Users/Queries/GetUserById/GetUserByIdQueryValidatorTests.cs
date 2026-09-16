using FluentValidation.TestHelper;
using SIGTI.Application.Features.Users.Queries.GetUserById;

namespace SIGTI.Application.Tests.Features.Users.Queries.GetUserById
{
    public class GetUserByIdQueryValidatorTests
    {
        private readonly GetUserByIdQueryValidator _validator;

        public GetUserByIdQueryValidatorTests()
        {
            _validator = new GetUserByIdQueryValidator();
        }

        [Fact]
        public void Validate_WhenIdIsValid_ShouldNotHaveValidationErrors()
        {
            var query = new GetUserByIdQuery(Guid.NewGuid());

            var result = _validator.TestValidate(query);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_WhenIdIsEmpty_ShouldHaveValidationErrors()
        {
            var query = new GetUserByIdQuery(Guid.Empty);

            var result = _validator.TestValidate(query);

            result
                .ShouldHaveValidationErrorFor(x => x.Id)
                .WithErrorMessage("O ID do usuário é obrigatório");
        }
    }
}

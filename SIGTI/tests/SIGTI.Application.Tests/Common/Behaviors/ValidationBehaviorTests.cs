using FluentValidation;
using FluentValidation.Results;
using Moq;
using Xunit;
using MediatR;
using SIGTI.Application.Common.Behaviors;
using FluentAssertions;

namespace SIGTI.Application.Tests.Common.Behaviors
{
    public sealed class ValidationBehaviorTests
    {
        public sealed record TestRequest(string Value);

        [Fact]
        public async Task Should_Throw_ValidationException_When_Request_Is_Invalid()
        {
            // Arrange
            var validator = new Mock<IValidator<TestRequest>>();

            var failures = new List<ValidationFailure>
            {
                new("Value", "O valor é obrigatório.")
            };

            validator.Setup(x => x.ValidateAsync(
                It.IsAny<IValidationContext>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(failures));

            var behavior = new ValidationBehavior<TestRequest, string>(
                new[] { validator.Object });

            var nextCalled = false;

            RequestHandlerDelegate<string> next = (cancellationToken) =>
            {
                nextCalled = true;
                return Task.FromResult("OK");
            };

            var request = new TestRequest("");

            // Act
            var action = () => behavior.Handle(
                request,
                next,
                CancellationToken.None);

            // Assert
            await action.Should()
                .ThrowAsync<ValidationException>();

            nextCalled.Should().BeFalse();

        }
    }
}

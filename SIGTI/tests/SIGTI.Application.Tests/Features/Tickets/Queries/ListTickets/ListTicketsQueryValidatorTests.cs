using FluentAssertions;
using SIGTI.Application.Features.Tickets.Queries.ListTickets;
using Xunit;

namespace SIGTI.Application.Tests.Features.Tickets.Queries.ListTickets;

public sealed class ListTicketsQueryValidatorTests
{
    private readonly ListTicketQueryValidator _validator = new();

    [Fact]
    public async Task Should_Validate_A_Valid_Query()
    {
        // Arrange
        var query = new ListTicketsQuery { Page = 1, PageSize = 20 };

        // Act
        var result = await _validator.ValidateAsync(query);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Should_Not_Allow_Page_Less_Than_One()
    {
        // Arrange
        var query = new ListTicketsQuery { Page = 0, PageSize = 20 };

        // Act
        var result = await _validator.ValidateAsync(query);

        // Assert
        result.IsValid.Should().BeFalse();

        result
            .Errors.Should()
            .ContainSingle(error =>
                error.PropertyName == nameof(ListTicketsQuery.Page)
                && error.ErrorMessage == "A página deve ser maior ou igual a 1."
            );
    }

    [Fact]
    public async Task Should_Not_Allow_PageSize_Less_Than_One()
    {
        // Arrange
        var query = new ListTicketsQuery { Page = 1, PageSize = 0 };

        // Act
        var result = await _validator.ValidateAsync(query);

        // Assert
        result.IsValid.Should().BeFalse();

        result
            .Errors.Should()
            .ContainSingle(error => error.PropertyName == nameof(ListTicketsQuery.PageSize));
    }

    [Fact]
    public async Task Should_Not_Allow_PageSize_Greater_Than_One_Hundred()
    {
        // Arrange
        var query = new ListTicketsQuery { Page = 1, PageSize = 101 };

        // Act
        var result = await _validator.ValidateAsync(query);

        // Assert
        result.IsValid.Should().BeFalse();

        result
            .Errors.Should()
            .ContainSingle(error =>
                error.PropertyName == nameof(ListTicketsQuery.PageSize)
                && error.ErrorMessage == "O tamanho da página deve estar entre 1 e 100."
            );
    }
}

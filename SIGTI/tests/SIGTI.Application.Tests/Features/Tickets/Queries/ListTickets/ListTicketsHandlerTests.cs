using FluentAssertions;
using Moq;
using SIGTI.Application.Common.Enums;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Application.Features.Tickets.Queries.ListTickets;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Enums;
using SIGTI.Domain.Tests.Builders;
using Xunit;

namespace SIGTI.Application.Features.Tickets.Queries.ListTickets
{
    public sealed class ListTicketsHandlerTests
    {
        [Fact]
        public async Task Should_Return_Paged_Tickets()
        {
            var repository = new Mock<ITicketRepository>();

            var ticket1 = new TicketBuilder().WithNumber(1).Build();

            var ticket2 = new TicketBuilder().WithNumber(2).Build();

            var tickets = new List<Ticket> { ticket1, ticket2 };

            repository
                .Setup(x =>
                    x.ListAsync(
                        It.IsAny<TicketListFilter>(),
                        It.IsAny<TicketSortField>(),
                        It.IsAny<SortDirection>(),
                        It.IsAny<int>(),
                        It.IsAny<int>(),
                        It.IsAny<CancellationToken>()
                    )
                )
                .ReturnsAsync(tickets);

            repository
                .Setup(x =>
                    x.CountAsync(It.IsAny<TicketListFilter>(), It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(2);

            var handler = new ListTicketsHandler(repository.Object);

            var query = new ListTicketsQuery { Page = 1, PageSize = 10 };

            var result = await handler.Handle(query, CancellationToken.None);

            result.Items.Should().HaveCount(2);
            result.TotalCount.Should().Be(2);
            result.Page.Should().Be(1);
            result.PageSize.Should().Be(10);
            result.TotalPages.Should().Be(1);
            result.HasPreviousPage.Should().BeFalse();
            result.HasNextPage.Should().BeFalse();
        }

        [Fact]
        public async Task Should_Calculate_Skip_From_Page_And_PageSize()
        {
            var repository = new Mock<ITicketRepository>();

            repository
                .Setup(x =>
                    x.ListAsync(
                        It.IsAny<TicketListFilter>(),
                        It.IsAny<TicketSortField>(),
                        It.IsAny<SortDirection>(),
                        It.IsAny<int>(),
                        It.IsAny<int>(),
                        It.IsAny<CancellationToken>()
                    )
                )
                .ReturnsAsync([]);

            repository
                .Setup(x =>
                    x.CountAsync(It.IsAny<TicketListFilter>(), It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(0);

            var handler = new ListTicketsHandler(repository.Object);

            var query = new ListTicketsQuery { Page = 2, PageSize = 10 };

            await handler.Handle(query, CancellationToken.None);

            repository.Verify(
                x =>
                    x.ListAsync(
                        It.IsAny<TicketListFilter>(),
                        It.IsAny<TicketSortField>(),
                        It.IsAny<SortDirection>(),
                        10,
                        10,
                        It.IsAny<CancellationToken>()
                    ),
                Times.Once
            );
        }

        [Fact]
        public async Task Should_Foward_Filter_To_Repository()
        {
            var repository = new Mock<ITicketRepository>();

            repository
                .Setup(x =>
                    x.ListAsync(
                        It.IsAny<TicketListFilter>(),
                        It.IsAny<TicketSortField>(),
                        It.IsAny<SortDirection>(),
                        It.IsAny<int>(),
                        It.IsAny<int>(),
                        It.IsAny<CancellationToken>()
                    )
                )
                .ReturnsAsync([]);

            repository
                .Setup(x =>
                    x.CountAsync(It.IsAny<TicketListFilter>(), It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(0);

            var handler = new ListTicketsHandler(repository.Object);

            var filter = new TicketListFilter { Priority = TicketPriority.Critical };

            var query = new ListTicketsQuery
            {
                Page = 1,
                PageSize = 20,
                Filter = filter,
            };

            await handler.Handle(query, CancellationToken.None);

            repository.Verify(
                x =>
                    x.ListAsync(
                        It.Is<TicketListFilter>(f => f.Priority == TicketPriority.Critical),
                        It.IsAny<TicketSortField>(),
                        It.IsAny<SortDirection>(),
                        0,
                        20,
                        It.IsAny<CancellationToken>()
                    ),
                Times.Once
            );
        }

        [Fact]
        public async Task Should_Foward_Sort_Options_To_Repository()
        {
            var repository = new Mock<ITicketRepository>();

            repository
                .Setup(x =>
                    x.ListAsync(
                        It.IsAny<TicketListFilter>(),
                        It.IsAny<TicketSortField>(),
                        It.IsAny<SortDirection>(),
                        It.IsAny<int>(),
                        It.IsAny<int>(),
                        It.IsAny<CancellationToken>()
                    )
                )
                .ReturnsAsync([]);

            repository
                .Setup(x =>
                    x.CountAsync(It.IsAny<TicketListFilter>(), It.IsAny<CancellationToken>())
                )
                .ReturnsAsync(0);

            var handler = new ListTicketsHandler(repository.Object);

            var query = new ListTicketsQuery
            {
                Page = 1,
                PageSize = 20,
                SortBy = TicketSortField.Priority,
                SortDirection = SortDirection.Ascending,
            };

            await handler.Handle(query, CancellationToken.None);

            repository.Verify(
                x =>
                    x.ListAsync(
                        It.IsAny<TicketListFilter>(),
                        TicketSortField.Priority,
                        SortDirection.Ascending,
                        0,
                        20,
                        It.IsAny<CancellationToken>()
                    ),
                Times.Once
            );
        }
    }
}

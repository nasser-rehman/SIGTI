using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SIGTI.API.Tests.Fixtures;
using SIGTI.Application.Features.Tickets.Commands.AddComment;
using SIGTI.Application.Features.Tickets.Commands.CloseTicket;
using SIGTI.Application.Features.Tickets.Commands.CreateTicket;
using SIGTI.Application.Features.Tickets.Commands.ResolveTicket;
using SIGTI.Application.Features.Tickets.Queries.GetTicketTimeline;
using SIGTI.Domain.Enums;
using SIGTI.Domain.Tests.Builders;

namespace SIGTI.API.Tests.Controllers
{
    [Collection("ApiTestCollection")]
    public class TicketLifecycleE2ETests : IAsyncLifetime
    {
        private readonly CustomWebApplicationFactory _factory;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() },
        };

        public TicketLifecycleE2ETests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        public Task DisposeAsync() => Task.CompletedTask;

        public async Task InitializeAsync()
        {
            await _factory.ResetDatabaseAsync();
        }

        [Fact]
        public async Task Ticket_FullLifecycle_ShouldSucceedAndRecordTimeline()
        {
            // Arrange: Actors setup (creator and tech)
            var (departmentId, queueId, technician, requester) =
                await _factory.ExecuteDbContextAsync(async context =>
                {
                    var dept = await context.Departments.FirstAsync();
                    var q = await context.SupportQueues.FirstAsync();
                    var tech = await context.Users.FirstAsync(u =>
                        u.Role == Role.Technician
                    );
                    var req = new UserBuilder()
                        .WithName("Carlos Solicitante")
                        .WithEmail("carlos@sigti.local")
                        .WithRole(Role.User)
                        .WithDepartment(dept)
                        .Build();

                    await context.Users.AddAsync(req);
                    await context.SaveChangesAsync();

                    return (dept.Id, q.Id, tech, req);
                });

            var requesterClient = _factory.CreateClientForUser(requester);
            var technicianClient = _factory.CreateClientForUser(technician);

            // ------------------------------------------------------------
            // 1. Create Ticket (POST /api/tickets)
            // ------------------------------------------------------------
            var createRequest = new CreateTicketRequest(
                "Monitor não liga",
                "O monitor da estação 42 não dá sinal de vídeo.",
                TicketPriority.High,
                TicketCategory.Hardware,
                departmentId,
                queueId
            );

            var createResponse = await requesterClient.PostAsJsonAsync(
                "/api/tickets",
                createRequest
            );

            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var createdTicket =
                await createResponse.Content.ReadFromJsonAsync<CreateTicketResponse>(
                    JsonOptions
                );

            createdTicket.Should().NotBeNull();
            createdTicket!.Id.Should().NotBeEmpty();
            createdTicket.Code.Should().StartWith("SIG-");
            createdTicket.Status.Should().Be(TicketStatus.Assigned);

            var ticketId = createdTicket.Id;

            // ------------------------------------------------------------
            // 2. Start Service (PATCH /api/tickets/{id}/start)
            // ------------------------------------------------------------
            var startResponse = await technicianClient.PatchAsync(
                $"/api/tickets/{ticketId}/start",
                null
            );
            startResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // ------------------------------------------------------------
            // 3. Add comments (POST /api/tickets/{id}/comments)
            // ------------------------------------------------------------
            var techCommentRequest = new AddCommentRequest(
                "Iniciando testes com novo cabo DisplayPort."
            );
            var techCommentResponse = await technicianClient.PostAsJsonAsync(
                $"/api/tickets/{ticketId}/comments",
                techCommentRequest
            );

            techCommentResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var userCommentRequest = new AddCommentRequest(
                "Obrigado pela agilidade!"
            );
            var userCommentResponse = await requesterClient.PostAsJsonAsync(
                $"/api/tickets/{ticketId}/comments",
                userCommentRequest
            );

            userCommentResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            // ------------------------------------------------------------
            // 4. Resolve Ticket (PATCH /api/tickets/{id}/resolve)
            // ------------------------------------------------------------
            var resolveResponse = await technicianClient.PatchAsync(
                $"/api/tickets/{ticketId}/resolve",
                null
            );
            resolveResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var resolvedTicket =
                await resolveResponse.Content.ReadFromJsonAsync<ResolveTicketResponse>(
                    JsonOptions
                );
            resolvedTicket.Should().NotBeNull();
            resolvedTicket!.Status.Should().Be(TicketStatus.Resolved);
            resolvedTicket.ResolvedAt.Should().NotBeNull();

            // ------------------------------------------------------------
            // 5. Close Ticket (PATCH /api/tickets/{id}/close)
            // ------------------------------------------------------------
            var closeResponse = await requesterClient.PatchAsync(
                $"/api/tickets/{ticketId}/close",
                null
            );
            closeResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var closedTicket =
                await closeResponse.Content.ReadFromJsonAsync<CloseTicketResponse>(
                    JsonOptions
                );
            closedTicket.Should().NotBeNull();
            closedTicket!.Status.Should().Be(TicketStatus.Closed);
            closedTicket.ClosedAt.Should().NotBeNull();

            // ------------------------------------------------------------
            // 6. Verify Timeline (GET /api/tickets/{id}/timeline)
            // ------------------------------------------------------------
            var timelineResponse = await requesterClient.GetAsync(
                $"/api/tickets/{ticketId}/timeline"
            );
            timelineResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var timeline =
                await timelineResponse.Content.ReadFromJsonAsync<TicketTimelineResponse>(
                    JsonOptions
                );
            timeline.Should().NotBeNull();
            timeline!.TicketId.Should().Be(ticketId);
            timeline.Status.Should().Be(TicketStatus.Closed);
            timeline.Timeline.Should().NotBeEmpty();

            timeline.Timeline.Should().Contain(e => e.EventType == "Created");
            timeline.Timeline.Should().Contain(e => e.EventType == "Assigned");
            timeline.Timeline.Should().Contain(e => e.EventType == "Started");
            timeline
                .Timeline.Should()
                .Contain(e => e.EventType == "CommentAdded");
            timeline.Timeline.Should().Contain(e => e.EventType == "Resolved");
            timeline.Timeline.Should().Contain(e => e.EventType == "Closed");

            // ------------------------------------------------------------
            // 7. Invariant Check: Terminal State (Don't permit comments after close the ticket)
            // ------------------------------------------------------------
            var commentAfterCloseResponse =
                await requesterClient.PostAsJsonAsync(
                    $"/api/tickets/{ticketId}/comments",
                    new AddCommentRequest(
                        "Tentando comentar após o fechamento..."
                    )
                );
            commentAfterCloseResponse
                .StatusCode.Should()
                .Be(HttpStatusCode.BadRequest);
        }
    }
}

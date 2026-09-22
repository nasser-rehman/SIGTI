using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SIGTI.API.Tests.Fixtures;
using SIGTI.Application.Features.SupportQueues.Commands.ActivateSupportQueue;
using SIGTI.Application.Features.SupportQueues.Commands.AddMember;
using SIGTI.Application.Features.SupportQueues.Commands.CreateSupportQueue;
using SIGTI.Application.Features.SupportQueues.Commands.DeactivateSupportQueue;
using SIGTI.Application.Features.SupportQueues.Commands.RemoveMember;
using SIGTI.Application.Features.SupportQueues.Commands.UpdateMemberCapacity;
using SIGTI.Application.Features.SupportQueues.Commands.UpdateSupportQueue;
using SIGTI.Application.Features.SupportQueues.Queries.GetSupportQueueById;
using SIGTI.Domain.Enums;
using Xunit;

namespace SIGTI.API.Tests.Controllers
{
    [Collection("ApiTestCollection")]
    public class SupportQueueManagementE2ETests : IAsyncLifetime
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() },
        };

        private readonly CustomWebApplicationFactory _factory;

        public SupportQueueManagementE2ETests(
            CustomWebApplicationFactory factory
        )
        {
            _factory = factory;
        }

        public Task DisposeAsync() => Task.CompletedTask;

        public async Task InitializeAsync()
        {
            await _factory.ResetDatabaseAsync();
        }

        [Fact]
        public async Task CreateSupportQueue_WhenCalledByAdmin_ShouldReturnCreatedWithLocationAndAllowGetById()
        {
            // Arrange
            var adminClient = _factory.CreateClientWithRole(Role.Administrator);
            var userClient = _factory.CreateClientWithRole(Role.User);

            var request = new CreateSupportQueueRequest(
                "Fila Redes e Conectividade",
                "Suporte a roteadores, switches e links dedicados."
            );

            // Act 1: POST /api/support-queues
            var createResponse = await adminClient.PostAsJsonAsync(
                "/api/support-queues",
                request
            );

            // Assert 1: 201 Created com Location header
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            createResponse.Headers.Location.Should().NotBeNull();

            var createdQueue =
                await createResponse.Content.ReadFromJsonAsync<CreateSupportQueueResponse>(
                    JsonOptions
                );
            createdQueue.Should().NotBeNull();
            createdQueue!.Id.Should().NotBeEmpty();

            // Act 2: GET /api/support-queues/{id}
            var getResponse = await userClient.GetAsync(
                $"/api/support-queues/{createdQueue.Id}"
            );

            // Assert 2: 200 OK com membros vazios
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var retrievedQueue =
                await getResponse.Content.ReadFromJsonAsync<GetSupportQueueByIdResponse>(
                    JsonOptions
                );
            retrievedQueue.Should().NotBeNull();
            retrievedQueue!.Id.Should().Be(createdQueue.Id);
            retrievedQueue.Name.Should().Be("Fila Redes e Conectividade");
            retrievedQueue
                .Description.Should()
                .Be("Suporte a roteadores, switches e links dedicados.");
            retrievedQueue.IsActive.Should().BeTrue();
            retrievedQueue.Members.Should().BeEmpty();
        }

        [Fact]
        public async Task MemberManagement_FullLifecycle_ShouldSucceed()
        {
            // Arrange
            var adminClient = _factory.CreateClientWithRole(Role.Administrator);

            // Obter técnico pré-semeado no banco
            var technician = await _factory.ExecuteDbContextAsync(
                async context =>
                {
                    return await context.Users.FirstAsync(u =>
                        u.Role == Role.Technician
                    );
                }
            );

            // 1. Criar Fila
            var createQueueResponse = await adminClient.PostAsJsonAsync(
                "/api/support-queues",
                new CreateSupportQueueRequest(
                    "Fila Teste Membros",
                    "Descrição Fila Membros"
                )
            );
            var queue =
                await createQueueResponse.Content.ReadFromJsonAsync<CreateSupportQueueResponse>(
                    JsonOptions
                );

            // 2. Adicionar Técnico (POST /api/support-queues/{id}/members)
            var addMemberResponse = await adminClient.PostAsJsonAsync(
                $"/api/support-queues/{queue!.Id}/members",
                new AddMemberRequest(technician.Id, 5)
            );
            addMemberResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // Verificar membro refletido no GetById
            var getAfterAddResponse = await adminClient.GetAsync(
                $"/api/support-queues/{queue.Id}"
            );
            var queueWithMember =
                await getAfterAddResponse.Content.ReadFromJsonAsync<GetSupportQueueByIdResponse>(
                    JsonOptions
                );
            queueWithMember!.Members.Should().HaveCount(1);
            var member = queueWithMember.Members.First();
            member.TechnicianId.Should().Be(technician.Id);
            member.MaxConcurrentTickets.Should().Be(5);
            member.IsActive.Should().BeTrue();

            // 3. Atualizar Capacidade do Técnico (PATCH /api/support-queues/{id}/members/{technicianId})
            var updateCapacityResponse = await adminClient.PatchAsJsonAsync(
                $"/api/support-queues/{queue.Id}/members/{technician.Id}",
                new UpdateMemberCapacityRequest(10)
            );
            updateCapacityResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var capacityResult =
                await updateCapacityResponse.Content.ReadFromJsonAsync<UpdateMemberCapacityResponse>(
                    JsonOptions
                );
            capacityResult!.MaxConcurrentTickets.Should().Be(10);

            // 4. Remover Técnico da Fila (DELETE /api/support-queues/{id}/members/{technicianId})
            var removeMemberResponse = await adminClient.DeleteAsync(
                $"/api/support-queues/{queue.Id}/members/{technician.Id}"
            );
            removeMemberResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var removeResult =
                await removeMemberResponse.Content.ReadFromJsonAsync<RemoveMemberResponse>(
                    JsonOptions
                );
            removeResult!.IsActive.Should().BeFalse();

            // Verificar estado final no GetById
            var getAfterRemoveResponse = await adminClient.GetAsync(
                $"/api/support-queues/{queue.Id}"
            );
            var finalQueue =
                await getAfterRemoveResponse.Content.ReadFromJsonAsync<GetSupportQueueByIdResponse>(
                    JsonOptions
                );
            finalQueue!.Members.First().IsActive.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateSupportQueue_WhenCalledByAdmin_ShouldUpdateAndPreventDuplicateName()
        {
            // Arrange
            var adminClient = _factory.CreateClientWithRole(Role.Administrator);

            var create1 = await adminClient.PostAsJsonAsync(
                "/api/support-queues",
                new CreateSupportQueueRequest("Fila Alfa", "Desc Alfa")
            );
            var queue1 =
                await create1.Content.ReadFromJsonAsync<CreateSupportQueueResponse>(
                    JsonOptions
                );

            var create2 = await adminClient.PostAsJsonAsync(
                "/api/support-queues",
                new CreateSupportQueueRequest("Fila Beta", "Desc Beta")
            );

            // Act 1: Atualização válida
            var updateResponse = await adminClient.PutAsJsonAsync(
                $"/api/support-queues/{queue1!.Id}",
                new UpdateSupportQueueRequest(
                    "Fila Alfa Renomeada",
                    "Nova Descrição Alfa"
                )
            );

            // Assert 1: 200 OK
            updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var updated =
                await updateResponse.Content.ReadFromJsonAsync<UpdateSupportQueueResponse>(
                    JsonOptions
                );
            updated!.Name.Should().Be("Fila Alfa Renomeada");
            updated.Description.Should().Be("Nova Descrição Alfa");

            // Act 2: Tentar atualizar para nome que já existe em outra fila ("FilaBeta")
            var duplicateResponse = await adminClient.PutAsJsonAsync(
                $"/api/support-queues/{queue1.Id}",
                new UpdateSupportQueueRequest(
                    "Fila Beta",
                    "Tentativa de duplicidade"
                )
            );

            // Assert 2: 400 Bad Request
            duplicateResponse
                .StatusCode.Should()
                .Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task DeactivateAndActivateSupportQueue_WhenCalledByAdmin_ShouldToggleStatus()
        {
            // Arrange
            var adminClient = _factory.CreateClientWithRole(Role.Administrator);

            var createResponse = await adminClient.PostAsJsonAsync(
                "/api/support-queues",
                new CreateSupportQueueRequest("Fila Status Test", "Desc")
            );
            var queue =
                await createResponse.Content.ReadFromJsonAsync<CreateSupportQueueResponse>(
                    JsonOptions
                );

            // Act 1: Desativar
            var deactivateResponse = await adminClient.PatchAsync(
                $"/api/support-queues/{queue!.Id}/deactivate",
                null
            );

            // Assert 1: 200 OK e IsActive = false
            deactivateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var deactivatedResult =
                await deactivateResponse.Content.ReadFromJsonAsync<DeactivateSupportQueueResponse>(
                    JsonOptions
                );
            deactivatedResult!.IsActive.Should().BeFalse();

            // Act 2: Reativar
            var activateResponse = await adminClient.PatchAsync(
                $"/api/support-queues/{queue.Id}/activate",
                null
            );

            // Assert 2: 200 OK e IsActive = true
            activateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var activatedResult =
                await activateResponse.Content.ReadFromJsonAsync<ActivateSupportQueueResponse>(
                    JsonOptions
                );
            activatedResult!.IsActive.Should().BeTrue();
        }

        [Fact]
        public async Task SupportQueueEndpoints_WhenCalledByNonAdmin_ShouldReturnForbidden()
        {
            // Arrange
            var userClient = _factory.CreateClientWithRole(Role.User);
            var queueId = Guid.NewGuid();
            var techId = Guid.NewGuid();

            // Act & Assert para operações administrativas
            var postQueue = await userClient.PostAsJsonAsync(
                "/api/support-queues",
                new CreateSupportQueueRequest("A", "B")
            );
            postQueue.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            var putQueue = await userClient.PutAsJsonAsync(
                $"/api/support-queues/{queueId}",
                new UpdateSupportQueueRequest("A", "B")
            );
            putQueue.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            var deactivateQueue = await userClient.PatchAsync(
                $"/api/support-queues/{queueId}/deactivate",
                null
            );
            deactivateQueue.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            var activateQueue = await userClient.PatchAsync(
                $"/api/support-queues/{queueId}/activate",
                null
            );
            activateQueue.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            var addMember = await userClient.PostAsJsonAsync(
                $"/api/support-queues/{queueId}/members",
                new AddMemberRequest(techId, 5)
            );
            addMember.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            var removeMember = await userClient.DeleteAsync(
                $"/api/support-queues/{queueId}/members/{techId}"
            );
            removeMember.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            var updateCapacity = await userClient.PatchAsJsonAsync(
                $"/api/support-queues/{queueId}/members/{techId}",
                new UpdateMemberCapacityRequest(5)
            );
            updateCapacity.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
    }
}

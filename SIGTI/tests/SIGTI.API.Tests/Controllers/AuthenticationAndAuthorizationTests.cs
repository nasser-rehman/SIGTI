using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using SIGTI.API.Tests.Fixtures;
using SIGTI.Application.Features.Departments.Commands.CreateDepartment;
using SIGTI.Domain.Enums;

namespace SIGTI.API.Tests.Controllers
{
    [Collection("ApiTestCollection")]
    public class AuthenticationAndAuthorizationTests : IAsyncLifetime
    {
        private readonly CustomWebApplicationFactory _factory;

        public AuthenticationAndAuthorizationTests(
            CustomWebApplicationFactory factory
        )
        {
            _factory = factory;
        }

        public async Task InitializeAsync()
        {
            await _factory.ResetDatabaseAsync();
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Theory]
        [InlineData("/api/tickets")]
        [InlineData("/api/departments")]
        [InlineData("/api/support-queues")]
        [InlineData("/api/users")]
        public async Task ProtectedEndpoints_WhenCalledWithoutToken_ShouldReturnUnauthorized(
            string endpoint
        )
        {
            // Arrange: cliente HTTP puro, sem cabeçalho Authorization
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(endpoint);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task CreateDepartment_WhenCalledByRegularUser_ShouldReturnForbidden()
        {
            // Arrange: cliente autenticado com Role.User
            var client = _factory.CreateClientWithRole(Role.User);
            var request = new CreateDepartmentRequest(
                "Financeiro E2E",
                "Setor financeiro"
            );

            // Act
            var response = await client.PostAsJsonAsync(
                "/api/departments",
                request
            );

            // Assert: deve ser barrado com 403 Forbidden
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task ListUsers_WhenCalledByRegularUser_ShouldReturnForbidden()
        {
            // Arrange: rota restrita a TechnicalStaff (Admin e Técnico)
            var client = _factory.CreateClientWithRole(Role.User);

            // Act
            var response = await client.GetAsync("/api/users");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task ListUsers_WhenCalledByTechnician_ShouldReturnOk()
        {
            // Arrange: cliente autenticado com Role.Technician
            var client = _factory.CreateClientWithRole(Role.Technician);

            // Act
            var response = await client.GetAsync("/api/users");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task CreateDepartment_WhenCalledByAdministrator_ShouldReturnCreated()
        {
            // Arrange: cliente autenticado com Role.Administrator
            var client = _factory.CreateClientWithRole(Role.Administrator);
            var uniqueName = $"RH E2E {Guid.NewGuid():N}";
            var request = new CreateDepartmentRequest(
                uniqueName,
                "Recursos Humanos criado no teste E2E"
            );

            // Act
            var response = await client.PostAsJsonAsync(
                "/api/departments",
                request
            );

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Headers.Location.Should().NotBeNull();
        }
    }
}

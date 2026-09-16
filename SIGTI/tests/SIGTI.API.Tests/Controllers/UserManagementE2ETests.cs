using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SIGTI.API.Tests.Fixtures;
using SIGTI.Application.Features.Users.Commands.CreateUser;
using SIGTI.Application.Features.Users.Commands.DeactivateUser;
using SIGTI.Application.Features.Users.Queries.GetUserById;
using SIGTI.Domain.Enums;
using SIGTI.Domain.Tests.Builders;

namespace SIGTI.API.Tests.Controllers
{
    [Collection("ApiTestCollection")]
    public class UserManagementE2ETests : IAsyncLifetime
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() },
        };

        private readonly CustomWebApplicationFactory _factory;

        public UserManagementE2ETests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        public async Task InitializeAsync()
        {
            await _factory.ResetDatabaseAsync();
        }

        public Task DisposeAsync() => Task.CompletedTask;

        [Fact]
        public async Task CreateUser_WhenCalledByAdmin_ShouldReturnCreatedAndAllowGetById()
        {
            // Arrange
            var department = await _factory.ExecuteDbContextAsync(
                async context => await context.Departments.FirstAsync()
            );

            var adminClient = _factory.CreateClientWithRole(Role.Administrator);
            var technicianClient = _factory.CreateClientWithRole(
                Role.Technician
            );

            var request = new CreateUserRequest(
                "Create User Request by admin",
                "user.new@sigti.local",
                "StrongPass@123",
                Role.Technician,
                department.Id
            );

            // Act 1: Create User by POST /api/users
            var createResponse = await adminClient.PostAsJsonAsync(
                "/api/users",
                request
            );

            // Assert 1: Should return 201 Created with location header
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            createResponse.Headers.Location.Should().NotBeNull();

            var createdUser =
                await createResponse.Content.ReadFromJsonAsync<CreateUserResponse>(
                    JsonOptions
                );
            createdUser.Should().NotBeNull();
            createdUser!.Id.Should().NotBeEmpty();
            createdUser.Name.Should().Be("Create User Request by admin");
            createdUser.Email.Should().Be("user.new@sigti.local");
            createdUser.Role.Should().Be(Role.Technician);
            createdUser.DepartmentId.Should().Be(department.Id);
            createdUser.IsActive.Should().BeTrue();

            // Act 2: Get details of new user via GET /api/users/{id}
            var getResponse = await technicianClient.GetAsync(
                $"/api/users/{createdUser.Id}"
            );

            // Assert 2: Technician can get with response 200 OK
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var retrievedUser =
                await getResponse.Content.ReadFromJsonAsync<GetUserByIdResponse>(
                    JsonOptions
                );
            retrievedUser.Should().NotBeNull();
            retrievedUser!.Id.Should().Be(createdUser.Id);
            retrievedUser.Name.Should().Be("Create User Request by admin");
            retrievedUser.Email.Should().Be("user.new@sigti.local");
            retrievedUser.IsActive.Should().BeTrue();
        }

        [Fact]
        public async Task CreateUser_WhenCalledByRegularUser_ShouldReturnForbidden()
        {
            // Arrange
            var department = await _factory.ExecuteDbContextAsync(
                async context => await context.Departments.FirstAsync()
            );

            var regularUserClient = _factory.CreateClientWithRole(Role.User);

            var request = new CreateUserRequest(
                "Common User Test",
                "common.user@sigti.local",
                "StrongPass@123",
                Role.Administrator,
                department.Id
            );

            // Act
            var response = await regularUserClient.PostAsJsonAsync(
                "/api/users",
                request
            );

            // Assert: RBAC should block with 403 Forbidden
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task DeactivateUser_WhenCalledByAdmin_ShouldDeactivateAndReturnOk()
        {
            // Arrange
            var (adminUser, targetUser) = await _factory.ExecuteDbContextAsync(
                async context =>
                {
                    var dept = await context.Departments.FirstAsync();

                    var admin = new UserBuilder()
                        .WithName("Administrator")
                        .WithEmail("admin@sigti.local")
                        .WithRole(Role.Administrator)
                        .WithDepartment(dept)
                        .Build();

                    var target = new UserBuilder()
                        .WithName("User To Be Deactivate")
                        .WithEmail("user@sigti.local")
                        .WithRole(Role.User)
                        .WithDepartment(dept)
                        .Build();

                    await context.Users.AddRangeAsync(admin, target);
                    await context.SaveChangesAsync();

                    return (admin, target);
                }
            );

            var adminClient = _factory.CreateClientWithRole(Role.Administrator);
            var staffClient = _factory.CreateClientWithRole(Role.Technician);

            // Act 1: Admin deactivate the common user via PATCH /api/users/{id}/deactivate
            var deactivateResponse = await adminClient.PatchAsync(
                $"/api/users/{targetUser.Id}/deactivate",
                null
            );

            // Assert 1: Should return Status Code 200 OK
            deactivateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var deactivateResult =
                await deactivateResponse.Content.ReadFromJsonAsync<DeactivateUserResponse>(
                    JsonOptions
                );
            deactivateResult.Should().NotBeNull();
            deactivateResult!.Id.Should().Be(targetUser.Id);
            deactivateResult.IsActive.Should().BeFalse();

            // Act 2: Check the profile to confirme inactive status in database
            var getResponse = await staffClient.GetAsync(
                $"/api/users/{targetUser.Id}"
            );
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var userAfterDeactivation =
                await getResponse.Content.ReadFromJsonAsync<GetUserByIdResponse>(
                    JsonOptions
                );
            userAfterDeactivation.Should().NotBeNull();
            userAfterDeactivation!.IsActive.Should().BeFalse();
        }

        [Fact]
        public async Task DeactivateUser_WhenCalledByRegularUser_ShouldReturnForbidden()
        {
            // Arrange
            var targetUser = await _factory.ExecuteDbContextAsync(
                async context =>
                    await context.Users.FirstAsync(u =>
                        u.Role == Role.Technician
                    )
            );

            var regularUserClient = _factory.CreateClientWithRole(Role.User);

            // Act
            var response = await regularUserClient.PatchAsync(
                $"/api/users/{targetUser.Id}/deactivate",
                null
            );

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
    }
}

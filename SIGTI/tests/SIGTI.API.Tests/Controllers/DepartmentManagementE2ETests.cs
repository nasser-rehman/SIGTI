using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SIGTI.API.Tests.Fixtures;
using SIGTI.Application.Features.Departments.Commands.CreateDepartment;
using SIGTI.Application.Features.Departments.Commands.UpdateDepartment;
using SIGTI.Application.Features.Departments.Queries.GetDepartmentById;
using SIGTI.Domain.Enums;
using SIGTI.Domain.Tests.Builders;

namespace SIGTI.API.Tests.Controllers
{
    [Collection("ApiTestCollection")]
    public class DepartmentManagementE2ETests : IAsyncLifetime
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() },
        };

        private readonly CustomWebApplicationFactory _factory;

        public DepartmentManagementE2ETests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        public Task DisposeAsync() => Task.CompletedTask;

        public async Task InitializeAsync()
        {
            await _factory.ResetDatabaseAsync();
        }

        [Fact]
        public async Task CreateDepartment_WhenCalledByAdmin_ShouldReturnCreatedWithLocationAndAllowGetById()
        {
            // Arrange
            var adminClient = _factory.CreateClientWithRole(Role.Administrator);
            var userClient = _factory.CreateClientWithRole(Role.User);

            var request = new CreateDepartmentRequest(
                "New Department By Admin",
                "New department created to test E2E"
            );

            // Act 1: Create Department POST /api/departments
            var createResponse = await adminClient.PostAsJsonAsync(
                "/api/departments",
                request
            );

            // Assert 1: Should return 201 Created with location header
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            createResponse.Headers.Location.Should().NotBeNull();

            var createdDepartment =
                await createResponse.Content.ReadFromJsonAsync<CreateDepartmentResponse>(
                    JsonOptions
                );

            createdDepartment.Should().NotBeNull();
            createdDepartment!.Id.Should().NotBeEmpty();
            createdDepartment.Name.Should().Be("New Department By Admin");
            createdDepartment
                .Description.Should()
                .Be("New department created to test E2E");
            createdDepartment.IsActive.Should().BeTrue();

            // Act 2: Get details of Department created via GET /api/departments/{id}
            var getResponse = await userClient.GetAsync(
                $"/api/departments/{createdDepartment.Id}"
            );

            // Assert 2: Any user logged should get status code 200
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var retrievedDepartment =
                await getResponse.Content.ReadFromJsonAsync<GetDepartmentByIdResponse>(
                    JsonOptions
                );

            retrievedDepartment.Should().NotBeNull();
            retrievedDepartment!.Id.Should().Be(createdDepartment.Id);
            retrievedDepartment.Name.Should().Be(createdDepartment.Name);
            retrievedDepartment
                .Description.Should()
                .Be(createdDepartment.Description);
            retrievedDepartment.IsActive.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateDepartment_WhenCalledByAdmin_ShouldUpdateAndReturnOk()
        {
            // Arrange
            var targetDepartment = await _factory.ExecuteDbContextAsync(
                async context =>
                {
                    var dept = new DepartmentBuilder()
                        .WithName("Cool Department")
                        .WithDescription("The coolest department ever.")
                        .Build();
                    await context.Departments.AddAsync(dept);
                    await context.SaveChangesAsync();

                    return dept;
                }
            );

            var adminClient = _factory.CreateClientWithRole(Role.Administrator);
            var request = new UpdateDepartmentRequest(
                "Not coolest anymore",
                "The department is going to be shut down."
            );

            // Act 1: Update department with admin client via PUT /api/departments/{id}
            var updateResponse = await adminClient.PutAsJsonAsync(
                $"/api/departments/{targetDepartment.Id}",
                request
            );

            // Assert 1: The request should return 200 OK and department details updated
            updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var updatedDepartment =
                await updateResponse.Content.ReadFromJsonAsync<UpdateDepartmentResponse>(
                    JsonOptions
                );
            updatedDepartment.Should().NotBeNull();
            updatedDepartment!.Id.Should().NotBeEmpty();
            updatedDepartment.Name.Should().Be("Not coolest anymore");
            updatedDepartment
                .Description.Should()
                .Be("The department is going to be shut down.");
            updatedDepartment.IsActive.Should().BeTrue();

            // Act 2: Get details of updated department via GET /api/departments/{id}
            var getResponse = await adminClient.GetAsync(
                $"/api/departments/{updatedDepartment.Id}"
            );

            // Assert 2: Get response should be 200 OK
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var retrievedDepartment =
                await getResponse.Content.ReadFromJsonAsync<GetDepartmentByIdResponse>(
                    JsonOptions
                );

            retrievedDepartment.Should().NotBeNull();
            retrievedDepartment!.Id.Should().Be(updatedDepartment.Id);
            retrievedDepartment.Name.Should().Be(updatedDepartment.Name);
            retrievedDepartment
                .Description.Should()
                .Be(updatedDepartment.Description);
            retrievedDepartment.IsActive.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateDepartment_WhenCalledByRegularUser_ShouldReturnForbidden()
        {
            // Arrange
            var department = await _factory.ExecuteDbContextAsync(async ctx =>
                await ctx.Departments.FirstAsync()
            );

            var userClient = _factory.CreateClientWithRole(Role.User);

            var request = new UpdateDepartmentRequest(
                "New name to this",
                department.Description
            );

            // Act
            var response = await userClient.PutAsJsonAsync(
                $"/api/departments/{department.Id}",
                request
            );

            // Assert: RBAC should block with 403 Forbidden
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task GetDepartmentById_WhenNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var adminClient = _factory.CreateClientWithRole(Role.Administrator);

            // Act
            var getResponse = await adminClient.GetAsync(
                $"/api/departments/{Guid.NewGuid()}"
            );

            // Assert: Not Found Exception 404
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UpdateDepartment_WhenDuplicateName_ShouldReturnBadRequest()
        {
            var (aDepartment, bDepartment) =
                await _factory.ExecuteDbContextAsync(async context =>
                {
                    var dptA = new DepartmentBuilder()
                        .WithName("Setor Alpha")
                        .Build();
                    var dptB = new DepartmentBuilder()
                        .WithName("Setor Beta")
                        .Build();

                    await context.Departments.AddRangeAsync(dptA, dptB);
                    await context.SaveChangesAsync();

                    return (dptA, dptB);
                });

            var adminClient = _factory.CreateClientWithRole(Role.Administrator);

            var request = new UpdateDepartmentRequest(
                "Setor Alpha",
                bDepartment.Description
            );

            // Act: Try to update with tha same name of department A
            var updateResponse = await adminClient.PutAsJsonAsync(
                $"/api/departments/{bDepartment.Id}",
                request
            );

            // Assert: Should return StatusCode 400 Bad Request
            updateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}

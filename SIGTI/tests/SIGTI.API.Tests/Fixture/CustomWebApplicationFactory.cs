using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Respawn;
using SIGTI.Application.Common.Interfaces.Services;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Enums;
using SIGTI.Domain.Tests.Builders;
using SIGTI.Infrastructure.Persistence.Context;
using SIGTI.Infrastructure.Persistence.Seed;

namespace SIGTI.API.Tests.Fixtures
{
    public class CustomWebApplicationFactory
        : WebApplicationFactory<Program>,
            IAsyncLifetime
    {
        private const string TestConnectString =
            "Host=localhost;Port=5432;Database=sigti_tests;Username=postgres;Password=postgres";

        private Respawner _respawner = default!;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d =>
                    d.ServiceType
                    == typeof(DbContextOptions<ApplicationDbContext>)
                );

                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseNpgsql(TestConnectString);
                });
            });

            builder.UseEnvironment("Testing");
        }

        public async Task InitializeAsync()
        {
            using var scope = Services.CreateScope();
            var context =
                scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await context.Database.MigrateAsync();

            await using var connection = new NpgsqlConnection(
                TestConnectString
            );
            await connection.OpenAsync();
            _respawner = await Respawner.CreateAsync(
                connection,
                new RespawnerOptions
                {
                    DbAdapter = DbAdapter.Postgres,
                    SchemasToInclude = ["public"],
                    TablesToIgnore = ["__EFMigrationsHistory"],
                }
            );
        }

        public async Task ResetDatabaseAsync()
        {
            await using var connection = new NpgsqlConnection(
                TestConnectString
            );
            await connection.OpenAsync();
            await _respawner.ResetAsync(connection);

            using var scope = Services.CreateScope();
            var context =
                scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await DatabaseSeeder.SeedAsync(context);
        }

        public HttpClient CreateClientWithRole(Role role)
        {
            var client = CreateClient();

            using var scope = Services.CreateScope();
            var tokenGenerator =
                scope.ServiceProvider.GetRequiredService<IJwtTokenGenerator>();

            var user = new UserBuilder().WithRole(role).Build();
            var (token, _) = tokenGenerator.GenerateToken(user);

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            return client;
        }

        public HttpClient CreateClientForUser(User user)
        {
            var client = CreateClient();

            using var scope = Services.CreateScope();
            var tokenGenerator =
                scope.ServiceProvider.GetRequiredService<IJwtTokenGenerator>();

            var (token, _) = tokenGenerator.GenerateToken(user);

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            return client;
        }

        public async Task<T> ExecuteDbContextAsync<T>(
            Func<ApplicationDbContext, Task<T>> action
        )
        {
            using var scope = Services.CreateScope();
            var context =
                scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            return await action(context);
        }

        public async Task ExecuteDbContextAsync(
            Func<ApplicationDbContext, Task> action
        )
        {
            using var scope = Services.CreateScope();
            var context =
                scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await action(context);
        }

        public new Task DisposeAsync() => Task.CompletedTask;
    }
}

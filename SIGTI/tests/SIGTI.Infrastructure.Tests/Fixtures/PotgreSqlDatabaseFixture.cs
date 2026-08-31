using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Respawn;
using SIGTI.Infrastructure.Persistence.Context;
using Xunit;

namespace SIGTI.Infrastructure.Tests.Fixtures;

public class PostgreSqlDatabaseFixture : IAsyncLifetime
{
    private const string ConnectionString =
        "Host=localhost;Port=5432;Database=sigti_tests;Username=postgres;Password=postgres";

    private Respawner _respawner = default!;

    public async Task InitializeAsync()
    {
        // 1. Aplica as migrations uma única vez na inicialização
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        await using var context = new ApplicationDbContext(options);
        await context.Database.MigrateAsync();

        // 2. Cria o Respawner usando uma conexão aberta
        await using var connection = new NpgsqlConnection(ConnectionString);
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

    public ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        return new ApplicationDbContext(options);
    }

    public async Task ResetDatabaseAsync()
    {
        // Abre uma conexão nova para executar o truncate do Respawn
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();
        await _respawner.ResetAsync(connection);
    }

    public Task DisposeAsync() => Task.CompletedTask;
}

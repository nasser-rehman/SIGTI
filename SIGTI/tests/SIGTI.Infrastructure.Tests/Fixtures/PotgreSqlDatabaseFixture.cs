using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Respawn;
using SIGTI.Infrastructure.Persistence.Context;
using Xunit;

namespace SIGTI.Infrastructure.Tests.Fixtures
{
    public class PostgreSqlDatabaseFixture : IAsyncLifetime
    {
        private const string ConnectionString =
            "Host=localhost;Port=5432;Database=sigti_tests;Username=postgres;Password=postgres";

        private Respawner _respawner = default!;
        private DbConnection _dbConnection = default;

        public async Task InitializeAsync()
        {
            // 1. Garante que o banco de teste existe e aplica as migrations
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(ConnectionString)
                .Options;

            await using var context = new ApplicationDbContext(options);
            await context.Database.MigrateAsync();

            // 2. Inicializa a conexão e o Respawner para limpezas rápidas
            _dbConnection = new NpgsqlConnection(ConnectionString);
            await _dbConnection.OpenAsync();

            _respawner = await Respawner.CreateAsync(
                _dbConnection,
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
            await _respawner.ResetAsync(_dbConnection);
        }

        public async Task DisposeAsync()
        {
            await _dbConnection.DisposeAsync();
        }
    }
}

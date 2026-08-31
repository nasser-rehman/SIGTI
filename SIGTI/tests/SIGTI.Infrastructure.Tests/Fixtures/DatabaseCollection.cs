using Xunit;

namespace SIGTI.Infrastructure.Tests.Fixtures
{
    [CollectionDefinition("DatabaseCollection")]
    public class DatabaseColletion : ICollectionFixture<PostgreSqlDatabaseFixture> { }
}

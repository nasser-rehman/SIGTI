using Microsoft.EntityFrameworkCore;
using SIGTI.Application.Common.Interfaces.Services;
using SIGTI.Infrastructure.Persistence.Context;

namespace SIGTI.Infrastructure.Services
{
    public class TicketNumberGenerator : ITicketNumberGenerator
    {
        private readonly ApplicationDbContext _context;

        public TicketNumberGenerator(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetNextAsync(CancellationToken cancellationToken)
        {
            var connection = _context.Database.GetDbConnection();

            if (connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync(cancellationToken);

            await using var command = connection.CreateCommand();

            command.CommandText = "SELECT nextval('ticket_number_sequence')";

            object? result = await command.ExecuteScalarAsync(cancellationToken);

            if (result is null)
                throw new InvalidOperationException(
                    "A sequence ticket_number_sequence não retornou nenhum valor."
                );

            return Convert.ToInt32(result);
        }
    }
}

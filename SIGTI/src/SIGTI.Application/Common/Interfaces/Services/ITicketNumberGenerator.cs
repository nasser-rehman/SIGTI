namespace SIGTI.Application.Common.Interfaces.Services;

public interface ITicketNumberGenerator
{
    Task<int> GetNextAsync(CancellationToken cancellationToken);
}

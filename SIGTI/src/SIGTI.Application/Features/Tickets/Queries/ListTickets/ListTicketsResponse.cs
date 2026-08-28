namespace SIGTI.Application.Features.Tickets.Queries.ListTickets
{
    public sealed record ListTicketsResponse(
        Guid Id,
        int Number,
        string Title,
        string Status,
        string Priority,
        string Category,
        string Department,
        string Queue,
        string? Technician,
        DateTime CreatedAt
    );
}

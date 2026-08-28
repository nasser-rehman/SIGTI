using SIGTI.Domain.Entities;
using SIGTI.Domain.Enums;

namespace SIGTI.Application.Features.Tickets.Queries.GetTicketById
{
    public sealed record GetTicketResponse(
        Guid Id,
        int Number,
        string Title,
        string Description,
        string Status,
        string Priority,
        string Category,
        string Department,
        string Queue,
        string? Technician,
        string CreatedBy,
        DateTime CreatedAt
    );
}

using MediatR;

namespace SIGTI.Application.Features.Departments.Queries.ListActiveDepartments
{
    public sealed record ListActiveDepartmentsQuery(
        bool IncludeInactive = false
    ) : IRequest<IReadOnlyCollection<ListActiveDepartmentsResponse>>;
}

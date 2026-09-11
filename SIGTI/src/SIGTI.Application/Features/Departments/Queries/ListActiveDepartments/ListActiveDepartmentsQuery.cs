using MediatR;

namespace SIGTI.Application.Features.Departments.Queries.ListActiveDepartments
{
    public sealed record ListActiveDepartmentsQuery
        : IRequest<IReadOnlyCollection<ListActiveDepartmentsResponse>>;
}

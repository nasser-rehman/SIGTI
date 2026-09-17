using MediatR;

namespace SIGTI.Application.Features.Departments.Queries.GetDepartmentById
{
    public sealed record GetDepartmentByIdQuery(Guid Id)
        : IRequest<GetDepartmentByIdResponse>;
}

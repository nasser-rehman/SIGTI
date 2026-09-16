using MediatR;

namespace SIGTI.Application.Features.Users.Queries.GetUserById
{
    public sealed record GetUserByIdQuery(Guid Id)
        : IRequest<GetUserByIdResponse>;
}

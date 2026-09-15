using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGTI.Application.Features.Users.Commands.CreateUser;
using SIGTI.Application.Features.Users.Queries.ListUsers;
using SIGTI.Domain.Constants;
using SIGTI.Domain.Enums;

namespace SIGTI.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly ISender _sender;

        public UsersController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        [Authorize(Roles = Roles.TechnicalStaff)]
        public async Task<IActionResult> List(
            [FromQuery] Role? role,
            CancellationToken cancellationToken
        )
        {
            var query = new ListUsersQuery(role);
            var result = await _sender.Send(query, cancellationToken);

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> Create(
            [FromBody] CreateUserRequest request,
            CancellationToken cancellationToken
        )
        {
            var command = new CreateUserCommand(
                request.Name,
                request.Email,
                request.Password,
                request.Role,
                request.DepartmentId
            );

            var result = await _sender.Send(command, cancellationToken);

            return StatusCode(StatusCodes.Status201Created, result);
        }
    }
}

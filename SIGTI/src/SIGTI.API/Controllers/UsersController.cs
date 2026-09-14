using MediatR;
using Microsoft.AspNetCore.Mvc;
using SIGTI.Application.Features.Users.Queries.ListUsers;
using SIGTI.Domain.Enums;

namespace SIGTI.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly ISender _sender;

        public UsersController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet]
        public async Task<IActionResult> List(
            [FromQuery] Role? role,
            CancellationToken cancellationToken
        )
        {
            var query = new ListUsersQuery(role);
            var result = await _sender.Send(query, cancellationToken);

            return Ok(result);
        }
    }
}

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGTI.Application.Common.Interfaces.Services;
using SIGTI.Application.Features.Users.Commands.ActivateUser;
using SIGTI.Application.Features.Users.Commands.ChangePassword;
using SIGTI.Application.Features.Users.Commands.CreateUser;
using SIGTI.Application.Features.Users.Commands.DeactivateUser;
using SIGTI.Application.Features.Users.Commands.ResetUserPassword;
using SIGTI.Application.Features.Users.Commands.UpdateUser;
using SIGTI.Application.Features.Users.Queries.GetUserById;
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
        private readonly ICurrentUserService _currentUserService;

        public UsersController(
            ISender sender,
            ICurrentUserService currentUserService
        )
        {
            _sender = sender;
            _currentUserService = currentUserService;
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

        [HttpGet("{id:guid}")]
        [Authorize(Roles = Roles.TechnicalStaff)]
        public async Task<IActionResult> GetById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken
        )
        {
            var response = await _sender.Send(
                new GetUserByIdQuery(id),
                cancellationToken
            );

            return Ok(response);
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

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                result
            );
        }

        [HttpPatch("{id:guid}/deactivate")]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> Deactivate(
            [FromRoute] Guid id,
            CancellationToken cancellationToken
        )
        {
            var response = await _sender.Send(
                new DeactivateUserCommand(id),
                cancellationToken
            );

            return Ok(response);
        }

        [HttpPatch("{id:guid}/activate")]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> Activate(
            [FromRoute] Guid id,
            CancellationToken cancellationToken
        )
        {
            var response = await _sender.Send(
                new ActivateUserCommand(id),
                cancellationToken
            );

            return Ok(response);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> Update(
            [FromRoute] Guid id,
            [FromBody] UpdateUserRequest request,
            CancellationToken cancellationToken
        )
        {
            var command = new UpdateUserCommand(
                id,
                request.Name,
                request.Role,
                request.DepartmentId
            );

            var response = await _sender.Send(command, cancellationToken);

            return Ok(response);
        }

        [HttpPatch("change-password")]
        public async Task<IActionResult> ChangePassword(
            [FromBody] ChangePasswordRequest request,
            CancellationToken cancellationToken
        )
        {
            var command = new ChangePasswordCommand(
                _currentUserService.UserId!.Value,
                request.CurrentPassword,
                request.NewPassword
            );

            await _sender.Send(command, cancellationToken);

            return NoContent();
        }

        [HttpPatch("{id:guid}/reset-password")]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> ResetPassword(
            [FromRoute] Guid id,
            [FromBody] ResetUserPasswordRequest request,
            CancellationToken cancellationToken
        )
        {
            var command = new ResetUserPasswordCommand(id, request.NewPassword);

            await _sender.Send(command, cancellationToken);

            return NoContent();
        }
    }
}

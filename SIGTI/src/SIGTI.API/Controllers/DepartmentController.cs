using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGTI.Application.Common.Interfaces.Services;
using SIGTI.Application.Features.Departments.Commands.ActivateDepartment;
using SIGTI.Application.Features.Departments.Commands.CreateDepartment;
using SIGTI.Application.Features.Departments.Commands.DeactivateDepartment;
using SIGTI.Application.Features.Departments.Commands.UpdateDepartment;
using SIGTI.Application.Features.Departments.Queries.GetDepartmentById;
using SIGTI.Application.Features.Departments.Queries.ListActiveDepartments;
using SIGTI.Domain.Constants;
using SIGTI.Domain.Enums;

namespace SIGTI.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/departments")]
    public class DepartmentController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly ICurrentUserService _currentUserService;

        public DepartmentController(
            ISender sender,
            ICurrentUserService currentUserService
        )
        {
            _sender = sender;
            _currentUserService = currentUserService;
        }

        [HttpPost]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> Create(
            [FromBody] CreateDepartmentRequest request,
            CancellationToken cancellationToken
        )
        {
            var command = new CreateDepartmentCommand(
                request.Name,
                request.Description
            );

            var result = await _sender.Send(command, cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                result
            );
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken
        )
        {
            var response = await _sender.Send(
                new GetDepartmentByIdQuery(id),
                cancellationToken
            );

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> ListActive(
            [FromQuery] bool includeInactive = false,
            CancellationToken cancellationToken = default
        )
        {
            if (
                includeInactive
                && !_currentUserService.IsInRole(Role.Administrator)
            )
                return Forbid();
            var response = await _sender.Send(
                new ListActiveDepartmentsQuery(includeInactive),
                cancellationToken
            );

            return Ok(response);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> Update(
            [FromRoute] Guid id,
            [FromBody] UpdateDepartmentRequest request,
            CancellationToken cancellationToken
        )
        {
            var command = new UpdateDepartmentCommand(
                id,
                request.Name,
                request.Description
            );

            var response = await _sender.Send(command, cancellationToken);

            return Ok(response);
        }

        [HttpPatch("{id:guid}/deactivate")]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> Deactivate(
            [FromRoute] Guid id,
            CancellationToken cancellationToken
        )
        {
            var response = await _sender.Send(
                new DeactivateDepartmentCommand(id),
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
                new ActivateDepartmentCommand(id),
                cancellationToken
            );

            return Ok(response);
        }
    }
}

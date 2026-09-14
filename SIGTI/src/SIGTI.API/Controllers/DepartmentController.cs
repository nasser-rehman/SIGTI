using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGTI.Application.Features.Departments.Commands.CreateDepartment;
using SIGTI.Application.Features.Departments.Queries.ListActiveDepartments;
using SIGTI.Domain.Constants;

namespace SIGTI.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/departments")]
    public class DepartmentController : ControllerBase
    {
        private readonly ISender _sender;

        public DepartmentController(ISender sender)
        {
            _sender = sender;
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
                nameof(ListActive),
                new { id = result.Id },
                result
            );
        }

        [HttpGet]
        public async Task<IActionResult> ListActive(
            CancellationToken cancellationToken
        )
        {
            var response = await _sender.Send(
                new ListActiveDepartmentsQuery(),
                cancellationToken
            );

            return Ok(response);
        }
    }
}

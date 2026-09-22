using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGTI.Application.Features.SupportQueues.Commands.ActivateSupportQueue;
using SIGTI.Application.Features.SupportQueues.Commands.AddMember;
using SIGTI.Application.Features.SupportQueues.Commands.CreateSupportQueue;
using SIGTI.Application.Features.SupportQueues.Commands.DeactivateSupportQueue;
using SIGTI.Application.Features.SupportQueues.Commands.RemoveMember;
using SIGTI.Application.Features.SupportQueues.Commands.UpdateMemberCapacity;
using SIGTI.Application.Features.SupportQueues.Commands.UpdateSupportQueue;
using SIGTI.Application.Features.SupportQueues.Queries.GetSupportQueueById;
using SIGTI.Application.Features.SupportQueues.Queries.ListActiveSupportQueues;
using SIGTI.Domain.Constants;
using SIGTI.Domain.Enums;

namespace SIGTI.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/support-queues")]
    public class SupportQueueController : ControllerBase
    {
        private readonly ISender _sender;

        public SupportQueueController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(
            [FromRoute] Guid id,
            CancellationToken cancellationToken
        )
        {
            var response = await _sender.Send(
                new GetSupportQueueByIdQuery(id),
                cancellationToken
            );
            return Ok(response);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> Update(
            [FromRoute] Guid id,
            [FromBody] UpdateSupportQueueRequest request,
            CancellationToken cancellationToken
        )
        {
            var command = new UpdateSupportQueueCommand(
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
            var command = new DeactivateSupportQueueCommand(id);
            var response = await _sender.Send(command, cancellationToken);

            return Ok(response);
        }

        [HttpPatch("{id:guid}/activate")]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> Activate(
            [FromRoute] Guid id,
            CancellationToken cancellationToken
        )
        {
            var command = new ActivateSupportQueueCommand(id);
            var response = await _sender.Send(command, cancellationToken);

            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> Create(
            [FromBody] CreateSupportQueueRequest request,
            CancellationToken cancellationToken
        )
        {
            var command = new CreateSupportQueueCommand(
                request.Name,
                request.Description
            );

            var response = await _sender.Send(command, cancellationToken);

            return CreatedAtAction(
                nameof(ListActive),
                new { id = response.Id },
                response
            );
        }

        [HttpPost("{id:guid}/members")]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> AddMemberToSupportQueue(
            [FromRoute] Guid id,
            [FromBody] AddMemberRequest request,
            CancellationToken cancellationToken
        )
        {
            var command = new AddMemberCommand(
                id,
                request.TechnicianId,
                request.MaxConcurrentTickets
            );

            var response = await _sender.Send(command, cancellationToken);

            return Ok(response);
        }

        [HttpDelete("{id:guid}/members/{technicianId:guid}")]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> RemoveMember(
            [FromRoute] Guid id,
            [FromRoute] Guid technicianId,
            CancellationToken cancellationToken
        )
        {
            var command = new RemoveMemberCommand(id, technicianId);
            var response = await _sender.Send(command, cancellationToken);

            return Ok(response);
        }

        [HttpPatch("{id:guid}/members/{technicianId:guid}")]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> UpdateMemberCapacity(
            [FromRoute] Guid id,
            [FromRoute] Guid technicianId,
            [FromBody] UpdateMemberCapacityRequest request,
            CancellationToken cancellationToken
        )
        {
            var command = new UpdateMemberCapacityCommand(
                id,
                technicianId,
                request.MaxConcurrentTickets
            );

            var response = await _sender.Send(command, cancellationToken);

            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> ListActive(
            CancellationToken cancellationToken
        )
        {
            var response = await _sender.Send(
                new ListActiveSupportQueuesQuery(),
                cancellationToken
            );

            return Ok(response);
        }
    }
}

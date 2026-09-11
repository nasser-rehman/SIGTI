using MediatR;
using Microsoft.AspNetCore.Mvc;
using SIGTI.Application.Features.SupportQueues.Commands.AddMember;
using SIGTI.Application.Features.SupportQueues.Commands.CreateSupportQueue;
using SIGTI.Application.Features.SupportQueues.Queries.ListActiveSupportQueues;

namespace SIGTI.API.Controllers
{
    [ApiController]
    [Route("api/support-queues")]
    public class SupportQueueController : ControllerBase
    {
        private readonly ISender _sender;

        public SupportQueueController(ISender sender)
        {
            _sender = sender;
        }

        [HttpPost]
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

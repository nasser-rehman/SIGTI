using MediatR;
using Microsoft.AspNetCore.Mvc;
using SIGTI.Application.Features.SupportQueues.Commands.AddMember;

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
    }
}

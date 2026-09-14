using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIGTI.Application.Common.Interfaces.Services;
using SIGTI.Application.Features.Tickets.Commands.AddComment;
using SIGTI.Application.Features.Tickets.Commands.CloseTicket;
using SIGTI.Application.Features.Tickets.Commands.CreateTicket;
using SIGTI.Application.Features.Tickets.Commands.DispatchTicket;
using SIGTI.Application.Features.Tickets.Commands.ResolveTicket;
using SIGTI.Application.Features.Tickets.Commands.StartTicketService;
using SIGTI.Application.Features.Tickets.Commands.TransferTicket;
using SIGTI.Application.Features.Tickets.Queries.GetTicketById;
using SIGTI.Application.Features.Tickets.Queries.GetTicketTimeline;
using SIGTI.Application.Features.Tickets.Queries.ListTicketComments;
using SIGTI.Application.Features.Tickets.Queries.ListTickets;
using SIGTI.Domain.Constants;

namespace SIGTI.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class TicketsController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly ICurrentUserService _currentUserService;

        public TicketsController(
            ISender sender,
            ICurrentUserService currentUserService
        )
        {
            _sender = sender;
            _currentUserService = currentUserService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateTicketRequest request,
            CancellationToken cancellationToken
        )
        {
            var command = new CreateTicketCommand(
                request.Title,
                request.Description,
                request.Priority,
                request.Category,
                request.DepartmentId,
                request.QueueId,
                _currentUserService.UserId!.Value
            );

            var result = await _sender.Send(command, cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.Id },
                result
            );
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _sender.Send(new GetTicketByIdQuery(id));
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> List(
            [FromQuery] ListTicketsQuery query,
            CancellationToken cancellationToken
        )
        {
            var response = await _sender.Send(query, cancellationToken);

            return Ok(response);
        }

        [HttpPatch("{id:guid}/start")]
        [Authorize(Roles = Roles.TechnicalStaff)]
        public async Task<IActionResult> Start(
            [FromRoute] Guid id,
            CancellationToken cancellationToken
        )
        {
            await _sender.Send(
                new StartTicketServiceCommand(id),
                cancellationToken
            );
            return NoContent();
        }

        [HttpPatch("{id:guid}/dispatch")]
        [Authorize(Roles = Roles.TechnicalStaff)]
        public async Task<IActionResult> Dispatch(
            [FromRoute] Guid id,
            [FromBody] DispatchTicketRequest request,
            CancellationToken cancellationToken
        )
        {
            var command = new DispatchTicketCommand(
                id,
                request.TechnicianId,
                _currentUserService.UserId!.Value,
                request.Reason
            );

            var response = await _sender.Send(command, cancellationToken);

            return Ok(response);
        }

        [HttpPatch("{id:guid}/resolve")]
        [Authorize(Roles = Roles.TechnicalStaff)]
        public async Task<IActionResult> Resolve(
            [FromRoute] Guid id,
            CancellationToken cancellationToken
        )
        {
            var response = await _sender.Send(
                new ResolveTicketCommand(id),
                cancellationToken
            );

            return Ok(response);
        }

        [HttpPatch("{id:guid}/close")]
        public async Task<IActionResult> Close(
            [FromRoute] Guid id,
            CancellationToken cancellationToken
        )
        {
            var response = await _sender.Send(
                new CloseTicketCommand(id),
                cancellationToken
            );

            return Ok(response);
        }

        [HttpPost("{id:guid}/comments")]
        public async Task<IActionResult> AddComment(
            [FromRoute] Guid id,
            [FromBody] AddCommentRequest request,
            CancellationToken cancellationToken
        )
        {
            var command = new AddCommentCommand(
                id,
                _currentUserService.UserId!.Value,
                request.Content
            );

            var response = await _sender.Send(command, cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = response.TicketId },
                response
            );
        }

        [HttpGet("{id:guid}/comments")]
        public async Task<IActionResult> GetComments(
            [FromRoute] Guid id,
            CancellationToken cancellationToken
        )
        {
            var response = await _sender.Send(
                new ListTicketCommentsQuery(id),
                cancellationToken
            );

            return Ok(response);
        }

        [HttpGet("{id:guid}/timeline")]
        public async Task<IActionResult> GetTimeline(
            [FromRoute] Guid id,
            CancellationToken cancellationToken
        )
        {
            var response = await _sender.Send(
                new GetTicketTimelineQuery(id),
                cancellationToken
            );

            return Ok(response);
        }

        [HttpPatch("{id:guid}/transfer")]
        [Authorize(Roles = Roles.TechnicalStaff)]
        public async Task<IActionResult> Transfer(
            [FromRoute] Guid id,
            [FromBody] TransferTicketRequest request,
            CancellationToken cancellationToken
        )
        {
            var command = new TransferTicketCommand(
                id,
                request.TargetQueueId,
                request.TargetTechnicianId,
                _currentUserService.UserId!.Value,
                request.Reason
            );

            var response = await _sender.Send(command, cancellationToken);

            return Ok(response);
        }
    }
}

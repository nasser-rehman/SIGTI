using MediatR;
using SIGTI.Application.Common.Exceptions;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Application.Common.Interfaces.Services;
using SIGTI.Domain.Entities;
using SIGTI.Domain.Factories;

namespace SIGTI.Application.Features.Tickets.Commands.CreateTicket
{
    public sealed class CreateTicketHandler
        : IRequestHandler<CreateTicketCommand, CreateTicketResponse>
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ITicketNumberGenerator _ticketNumberGenerator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TicketFactory _ticketFactory;

        private readonly ITechnicianAssignmentService _assignmentService;
        private readonly IEntityReferenceService _entityReferenceService;

        public CreateTicketHandler(
            ITicketRepository ticketRepository,
            ITicketNumberGenerator ticketNumberGenerator,
            IUnitOfWork unitOfWork,
            TicketFactory ticketFactory,
            IEntityReferenceService entityReferenceService,
            ITechnicianAssignmentService assignmentService
        )
        {
            _ticketRepository = ticketRepository;
            _ticketNumberGenerator = ticketNumberGenerator;
            _unitOfWork = unitOfWork;
            _ticketFactory = ticketFactory;
            _entityReferenceService = entityReferenceService;
            _assignmentService = assignmentService;
        }

        public async Task<CreateTicketResponse> Handle(
            CreateTicketCommand request,
            CancellationToken cancellationToken
        )
        {
            // 1 - Buscar usuário
            var user = await _entityReferenceService.GetRequiredUserAsync(
                request.CreatedById,
                cancellationToken
            );
            // 3 - Buscar departamento
            var department = await _entityReferenceService.GetRequiredDepartmentAsync(
                request.DepartmentId,
                cancellationToken
            );
            // 5 - Buscar fila
            var queue = await _entityReferenceService.GetRequiredQueueAsync(
                request.QueueId,
                cancellationToken
            );

            var selectedMember = await _assignmentService.SelectTechnicianAsync(
                queue,
                cancellationToken
            );
            // 7 - Gerar número do chamado
            var ticketNumber = await _ticketNumberGenerator.GetNextAsync(cancellationToken);

            var systemUser = await _entityReferenceService.GetRequiredSystemUserAsync(
                cancellationToken
            );

            // 8 - Criar Ticket
            var ticket = _ticketFactory.Create(
                ticketNumber,
                request.Title,
                request.Description,
                request.Priority,
                request.Category,
                department,
                user,
                queue
            );

            ticket.AssignTechnician(
                selectedMember.Technician,
                systemUser,
                "Atribuição automática do sistema!"
            );

            // 9 - Adicionar Ticket ao Repository
            await _ticketRepository.AddAsync(ticket, cancellationToken);
            // 10 - Salvar alterações (UnitOfWork)
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            // 11 - Retornar Response
            return new CreateTicketResponse(ticket.Id, ticket.Code, ticket.Title, ticket.Status);
        }
    }
}

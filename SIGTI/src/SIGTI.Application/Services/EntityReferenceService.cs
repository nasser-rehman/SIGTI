using SIGTI.Application.Common.Exceptions;
using SIGTI.Application.Common.Interfaces.Persistence;
using SIGTI.Application.Common.Interfaces.Services;
using SIGTI.Domain.Entities;

namespace SIGTI.Application.Common.Services
{
    public sealed class EntityReferenceService : IEntityReferenceService
    {
        private readonly IUserRepository _userRepository;

        private readonly IDepartmentRepository _departmentRepository;

        private readonly ISupportQueueRepository _supportQueueRepository;

        private readonly ITicketRepository _ticketRepository;

        public EntityReferenceService(
            IUserRepository userRepository,
            IDepartmentRepository departmentRepository,
            ISupportQueueRepository supportQueueRepository,
            ITicketRepository ticketRepository
        )
        {
            _userRepository = userRepository;
            _departmentRepository = departmentRepository;
            _supportQueueRepository = supportQueueRepository;
            _ticketRepository = ticketRepository;
        }

        public async Task<User> GetRequiredUserAsync(Guid id, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(id, cancellationToken);

            if (user is null)
                throw new NotFoundException($"Usuãrio'{id}' não encontrado.");
            return user;
        }

        public async Task<Department> GetRequiredDepartmentAsync(
            Guid id,
            CancellationToken cancellationToken
        )
        {
            var department = await _departmentRepository.GetByIdAsync(id, cancellationToken);

            if (department is null)
                throw new NotFoundException($"Departamento com '{id}' não encontrado.");

            return department;
        }

        public async Task<SupportQueue> GetRequiredQueueAsync(
            Guid id,
            CancellationToken cancellationToken
        )
        {
            var queue = await _supportQueueRepository.GetByIdAsync(id, cancellationToken);

            if (queue is null)
                throw new NotFoundException($"Fila de suporte com ${id} não encontrado.");
            return queue;
        }

        public async Task<Ticket> GetRequiredTicketAsync(
            Guid id,
            CancellationToken cancellationToken
        )
        {
            var ticket = await _ticketRepository.GetByIdAsync(id, cancellationToken);

            if (ticket is null)
                throw new NotFoundException($"Ticket com ID: ${id} não encontrado.");
            return ticket;
        }

        public async Task<User> GetRequiredSystemUserAsync(CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetSystemUserAsync(cancellationToken);

            if (user is null)
                throw new NotFoundException("Usuário do sistema não encontrado.");

            return user;
        }
    }
}

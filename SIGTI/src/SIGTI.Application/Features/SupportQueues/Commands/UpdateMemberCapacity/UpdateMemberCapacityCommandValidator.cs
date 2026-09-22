using FluentValidation;

namespace SIGTI.Application.Features.SupportQueues.Commands.UpdateMemberCapacity
{
    public sealed class UpdateMemberCapacityCommandValidator
        : AbstractValidator<UpdateMemberCapacityCommand>
    {
        public UpdateMemberCapacityCommandValidator()
        {
            RuleFor(x => x.QueueId)
                .NotEmpty()
                .WithMessage(
                    "O identificador da fila de suporte é obrigatório."
                );

            RuleFor(x => x.TechnicianId)
                .NotEmpty()
                .WithMessage("O identificador do técnico é obrigatório.");

            RuleFor(x => x.MaxConcurrentTickets)
                .GreaterThan(0)
                .WithMessage(
                    "O limite de chamados simultâneos deve ser maior que zero."
                );
        }
    }
}

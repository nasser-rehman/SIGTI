using FluentValidation;

namespace SIGTI.Application.Features.SupportQueues.Commands.RemoveMember
{
    public sealed class RemoveMemberCommandValidator
        : AbstractValidator<RemoveMemberCommand>
    {
        public RemoveMemberCommandValidator()
        {
            RuleFor(x => x.QueueId)
                .NotEmpty()
                .WithMessage(
                    "O identificador da fila de suporte é obrigatório."
                );

            RuleFor(x => x.TechnicianId)
                .NotEmpty()
                .WithMessage("O identificador do técnico é obrigatório.");
        }
    }
}

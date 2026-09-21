using FluentValidation;

namespace SIGTI.Application.Features.SupportQueues.Commands.DeactivateSupportQueue
{
    public sealed class DeactivateSupportQueueCommandValidator
        : AbstractValidator<DeactivateSupportQueueCommand>
    {
        public DeactivateSupportQueueCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage(
                    "O identificador da fila de suporte é obrigatório."
                );
        }
    }
}

using FluentValidation;

namespace SIGTI.Application.Features.SupportQueues.Commands.ActivateSupportQueue
{
    public sealed class ActivateSupportQueueCommandValidator
        : AbstractValidator<ActivateSupportQueueCommand>
    {
        public ActivateSupportQueueCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage(
                    "O identificador da fila de suporte é obrigatório."
                );
        }
    }
}

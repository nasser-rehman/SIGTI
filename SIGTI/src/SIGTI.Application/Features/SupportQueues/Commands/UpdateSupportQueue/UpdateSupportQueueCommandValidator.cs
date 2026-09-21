using FluentValidation;

namespace SIGTI.Application.Features.SupportQueues.Commands.UpdateSupportQueue
{
    public sealed class UpdateSupportQueueCommandValidator
        : AbstractValidator<UpdateSupportQueueCommand>
    {
        public UpdateSupportQueueCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage(
                    "O identificador da fila de suporte é obrigatório."
                );

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("O nome da fila de suporte é obrigatório.")
                .MaximumLength(150)
                .WithMessage(
                    "O nome da fila de suporte deve ter no máximo 150 caracteres."
                );

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("A descrição da fila de suporte é obrigatória.")
                .MaximumLength(500)
                .WithMessage(
                    "A descrição da fila de suporte deve ter no máximo 500 caracteres."
                );
        }
    }
}

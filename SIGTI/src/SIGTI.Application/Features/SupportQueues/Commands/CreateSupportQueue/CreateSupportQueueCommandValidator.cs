using FluentValidation;

namespace SIGTI.Application.Features.SupportQueues.Commands.CreateSupportQueue
{
    public sealed class CreateSupportQueueCommandValidator
        : AbstractValidator<CreateSupportQueueCommand>
    {
        public CreateSupportQueueCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("O nome da fila é obrigatório.")
                .MaximumLength(150)
                .WithMessage("O tamanho máximo de nome é de 150 caracteres.");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("A descrição para a fila é obrigatória.")
                .MaximumLength(500)
                .WithMessage(
                    "Tamanho máximo de descrição é de 500 caracteres."
                );
        }
    }
}

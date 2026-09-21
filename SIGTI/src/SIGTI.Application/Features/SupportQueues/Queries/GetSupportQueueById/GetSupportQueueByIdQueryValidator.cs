using FluentValidation;

namespace SIGTI.Application.Features.SupportQueues.Queries.GetSupportQueueById
{
    public sealed class GetSupportQueueByIdQueryValidator
        : AbstractValidator<GetSupportQueueByIdQuery>
    {
        public GetSupportQueueByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage(
                    "O identificador da fila de suporte é obrigatório."
                );
        }
    }
}

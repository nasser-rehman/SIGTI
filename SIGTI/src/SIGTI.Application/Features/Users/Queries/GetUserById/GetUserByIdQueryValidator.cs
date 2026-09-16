using FluentValidation;

namespace SIGTI.Application.Features.Users.Queries.GetUserById
{
    public sealed class GetUserByIdQueryValidator
        : AbstractValidator<GetUserByIdQuery>
    {
        public GetUserByIdQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("O ID do usuário é obrigatório");
        }
    }
}

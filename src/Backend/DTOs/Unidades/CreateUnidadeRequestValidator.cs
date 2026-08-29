using FluentValidation;
using Rte.Api.DTOs.Unidades;

namespace Rte.Api.DTOs.Unidades;

public class CreateUnidadeRequestValidator : AbstractValidator<CreateUnidadeRequest>
{
    public CreateUnidadeRequestValidator()
    {
        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("O código da unidade é obrigatório.")
            .MaximumLength(20);

        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .MaximumLength(120);

        RuleFor(x => x.Status).IsInEnum().WithMessage("Status inválido. Utilize Ativo ou Inativo.");
    }
}

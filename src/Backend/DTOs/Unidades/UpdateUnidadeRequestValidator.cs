using FluentValidation;
using Rte.Api.DTOs.Unidades;

namespace Rte.Api.DTOs.Unidades;

public class UpdateUnidadeRequestValidator : AbstractValidator<UpdateUnidadeRequest>
{
    public UpdateUnidadeRequestValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .MaximumLength(120);

        RuleFor(x => x.Status).IsInEnum().WithMessage("Status inválido. Utilize Ativo ou Inativo.");
    }
}

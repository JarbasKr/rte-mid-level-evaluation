using FluentValidation;
using Rte.Api.DTOs.Colaboradores;

namespace Rte.Api.DTOs.Colaboradores;

public class UpdateColaboradorRequestValidator : AbstractValidator<UpdateColaboradorRequest>
{
    public UpdateColaboradorRequestValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .MaximumLength(120);

        RuleFor(x => x.UnidadeId).NotEmpty().WithMessage("A unidade é obrigatória.");
    }
}

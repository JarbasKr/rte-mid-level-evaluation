using FluentValidation;
using Rte.Api.DTOs.Colaboradores;

namespace Rte.Api.DTOs.Colaboradores;

public class CreateColaboradorRequestValidator : AbstractValidator<CreateColaboradorRequest>
{
    public CreateColaboradorRequestValidator()
    {
        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("O código é obrigatório.")
            .MaximumLength(20);

        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .MaximumLength(120);

        RuleFor(x => x.UnidadeId).NotEmpty().WithMessage("A unidade é obrigatória.");
        RuleFor(x => x.UsuarioId).NotEmpty().WithMessage("O usuário associado é obrigatório.");
    }
}

using FluentValidation;
using Rte.Api.DTOs.Usuarios;

namespace Rte.Api.DTOs.Usuarios;

public class UpdateUsuarioRequestValidator : AbstractValidator<UpdateUsuarioRequest>
{
    public UpdateUsuarioRequestValidator()
    {
        RuleFor(x => x.Status).IsInEnum().WithMessage("Status inválido. Utilize Ativo ou Inativo.");
        RuleFor(x => x.Senha)
            .MinimumLength(6).WithMessage("A senha deve ter no mínimo 6 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.Senha));
    }
}

using FluentValidation;
using Rte.Api.DTOs.Usuarios;

namespace Rte.Api.DTOs.Usuarios;

public class CreateUsuarioRequestValidator : AbstractValidator<CreateUsuarioRequest>
{
    public CreateUsuarioRequestValidator()
    {
        RuleFor(x => x.Codigo)
            .NotEmpty().WithMessage("O código é obrigatório.")
            .MaximumLength(20).WithMessage("O código deve ter no máximo 20 caracteres.");

        RuleFor(x => x.Login)
            .NotEmpty().WithMessage("O login é obrigatório.")
            .MinimumLength(3).WithMessage("O login deve ter no mínimo 3 caracteres.")
            .MaximumLength(80).WithMessage("O login deve ter no máximo 80 caracteres.");

        RuleFor(x => x.Senha)
            .NotEmpty().WithMessage("A senha é obrigatória.")
            .MinimumLength(6).WithMessage("A senha deve ter no mínimo 6 caracteres.");

        RuleFor(x => x.Status).IsInEnum().WithMessage("Status inválido. Utilize Ativo ou Inativo.");
    }
}

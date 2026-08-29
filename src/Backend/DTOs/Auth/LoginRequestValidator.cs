using FluentValidation;
using Rte.Api.DTOs.Auth;

namespace Rte.Api.DTOs.Auth;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Login).NotEmpty().WithMessage("O login é obrigatório.");
        RuleFor(x => x.Senha).NotEmpty().WithMessage("A senha é obrigatória.");
    }
}

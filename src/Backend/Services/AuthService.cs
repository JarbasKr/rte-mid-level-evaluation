using Microsoft.EntityFrameworkCore;
using Rte.Api.Authentication;
using Rte.Api.DTOs.Auth;
using Rte.Api.Entities;
using Rte.Api.Exceptions;
using Rte.Api.Repositories;

namespace Rte.Api.Services;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}

public class AuthService : IAuthService
{
    private readonly IRepository<Usuario> _usuarios;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(
        IRepository<Usuario> usuarios,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService)
    {
        _usuarios = usuarios;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarios.Query()
            .FirstOrDefaultAsync(x => x.Login == request.Login.Trim(), cancellationToken);

        var credenciaisInvalidas = usuario is null || !_passwordHasher.Verify(request.Senha, usuario.SenhaHash);
        if (credenciaisInvalidas)
        {
            throw new UnauthorizedException("Credenciais inválidas.");
        }

        if (!usuario!.EstaAtiva())
        {
            throw new UnauthorizedException("Usuário inativo. Entre em contato com o administrador.");
        }

        var (token, expiraEm) = _jwtTokenService.Generate(usuario);

        return new LoginResponse
        {
            Token = token,
            Expiration = expiraEm,
            ExpiraEm = expiraEm,
            Login = usuario.Login,
            User = new AuthUserResponse
            {
                Id = usuario.Id,
                Name = usuario.Login,
                Email = usuario.Login
            }
        };
    }
}

using Microsoft.EntityFrameworkCore;
using Rte.Api.Authentication;
using Rte.Api.DTOs.Usuarios;
using Rte.Api.Entities;
using Rte.Api.Exceptions;
using Rte.Api.Repositories;

namespace Rte.Api.Services;

public interface IUsuarioService
{
    Task<IReadOnlyList<UsuarioResponse>> ListarAsync(StatusRegistro? status, CancellationToken cancellationToken = default);
    Task<UsuarioResponse> ObterPorCodigoAsync(string codigo, CancellationToken cancellationToken = default);
    Task<UsuarioResponse> CriarAsync(CreateUsuarioRequest request, CancellationToken cancellationToken = default);
    Task<UsuarioResponse> AtualizarAsync(string codigo, UpdateUsuarioRequest request, CancellationToken cancellationToken = default);
}

public class UsuarioService : IUsuarioService
{
    private readonly IRepository<Usuario> _usuarios;
    private readonly IPasswordHasher _passwordHasher;

    public UsuarioService(IRepository<Usuario> usuarios, IPasswordHasher passwordHasher)
    {
        _usuarios = usuarios;
        _passwordHasher = passwordHasher;
    }

    public async Task<IReadOnlyList<UsuarioResponse>> ListarAsync(StatusRegistro? status, CancellationToken cancellationToken = default)
    {
        var query = _usuarios.Query();

        if (status.HasValue)
        {
            query = query.Where(x => x.Status == status.Value);
        }

        return await query
            .OrderBy(x => x.Login)
            .Select(x => new UsuarioResponse
            {
                Id = x.Id,
                Codigo = x.Codigo,
                Login = x.Login,
                Status = x.Status
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<UsuarioResponse> ObterPorCodigoAsync(string codigo, CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarios.Query()
            .FirstOrDefaultAsync(x => x.Codigo == codigo.Trim().ToUpperInvariant(), cancellationToken)
            ?? throw new NotFoundException($"Usuário '{codigo}' não encontrado.");

        return Map(usuario);
    }

    public async Task<UsuarioResponse> CriarAsync(CreateUsuarioRequest request, CancellationToken cancellationToken = default)
    {
        var codigo = request.Codigo.Trim().ToUpperInvariant();
        var login = request.Login.Trim();

        if (await _usuarios.Query().AnyAsync(x => x.Codigo == codigo, cancellationToken))
        {
            throw new ConflictException($"Já existe um usuário com o código '{codigo}'.");
        }

        if (await _usuarios.Query().AnyAsync(x => x.Login.ToLower() == login.ToLower(), cancellationToken))
        {
            throw new ConflictException($"Já existe um usuário com o login '{login}'.");
        }

        var usuario = new Usuario
        {
            Codigo = codigo,
            Login = login,
            SenhaHash = _passwordHasher.Hash(request.Senha),
            Status = request.Status
        };

        await _usuarios.AddAsync(usuario, cancellationToken);
        await _usuarios.SaveChangesAsync(cancellationToken);

        return Map(usuario);
    }

    public async Task<UsuarioResponse> AtualizarAsync(string codigo, UpdateUsuarioRequest request, CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarios.Query()
            .FirstOrDefaultAsync(x => x.Codigo == codigo.Trim().ToUpperInvariant(), cancellationToken);

        if (usuario is null)
        {
            throw new NotFoundException($"Usuário '{codigo}' não encontrado.");
        }

        usuario.Status = request.Status;

        if (!string.IsNullOrWhiteSpace(request.Senha))
        {
            usuario.SenhaHash = _passwordHasher.Hash(request.Senha);
        }

        _usuarios.Update(usuario);
        await _usuarios.SaveChangesAsync(cancellationToken);

        return Map(usuario);
    }

    private static UsuarioResponse Map(Usuario usuario) => new()
    {
        Id = usuario.Id,
        Codigo = usuario.Codigo,
        Login = usuario.Login,
        Status = usuario.Status
    };
}

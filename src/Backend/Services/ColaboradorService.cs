using Microsoft.EntityFrameworkCore;
using Rte.Api.DTOs.Colaboradores;
using Rte.Api.Entities;
using Rte.Api.Exceptions;
using Rte.Api.Repositories;

namespace Rte.Api.Services;

public interface IColaboradorService
{
    Task<IReadOnlyList<ColaboradorResponse>> ListarAsync(CancellationToken cancellationToken = default);
    Task<ColaboradorResponse> ObterPorCodigoAsync(string codigo, CancellationToken cancellationToken = default);
    Task<ColaboradorResponse> CriarAsync(CreateColaboradorRequest request, CancellationToken cancellationToken = default);
    Task<ColaboradorResponse> AtualizarAsync(string codigo, UpdateColaboradorRequest request, CancellationToken cancellationToken = default);
    Task RemoverAsync(string codigo, CancellationToken cancellationToken = default);
}

public class ColaboradorService : IColaboradorService
{
    private readonly IRepository<Colaborador> _colaboradores;
    private readonly IRepository<Unidade> _unidades;
    private readonly IRepository<Usuario> _usuarios;

    public ColaboradorService(
        IRepository<Colaborador> colaboradores,
        IRepository<Unidade> unidades,
        IRepository<Usuario> usuarios)
    {
        _colaboradores = colaboradores;
        _unidades = unidades;
        _usuarios = usuarios;
    }

    public async Task<IReadOnlyList<ColaboradorResponse>> ListarAsync(CancellationToken cancellationToken = default)
    {
        var colaboradores = await QueryComRelacionamentos()
            .OrderBy(x => x.Codigo)
            .ToListAsync(cancellationToken);

        return colaboradores.Select(Map).ToList();
    }

    public async Task<ColaboradorResponse> ObterPorCodigoAsync(string codigo, CancellationToken cancellationToken = default)
    {
        var colaborador = await QueryComRelacionamentos()
            .FirstOrDefaultAsync(x => x.Codigo == codigo.Trim().ToUpperInvariant(), cancellationToken)
            ?? throw new NotFoundException($"Colaborador '{codigo}' não encontrado.");

        return Map(colaborador);
    }

    public async Task<ColaboradorResponse> CriarAsync(CreateColaboradorRequest request, CancellationToken cancellationToken = default)
    {
        var codigo = request.Codigo.Trim().ToUpperInvariant();

        if (await _colaboradores.Query().AnyAsync(x => x.Codigo == codigo, cancellationToken))
        {
            throw new ConflictException($"Já existe um colaborador com o código '{codigo}'.");
        }

        var unidade = await ObterUnidadeParaAssociacaoAsync(request.UnidadeId, cancellationToken);
        var usuario = await _usuarios.GetByIdAsync(request.UsuarioId, cancellationToken)
            ?? throw new NotFoundException("Usuário associado não encontrado.");

        if (!usuario.EstaAtiva())
        {
            throw new BusinessRuleException("Não é possível associar um colaborador a um usuário inativo.");
        }

        if (await _colaboradores.Query().AnyAsync(x => x.UsuarioId == usuario.Id, cancellationToken))
        {
            throw new ConflictException("Este usuário já está associado a outro colaborador.");
        }

        var colaborador = new Colaborador
        {
            Codigo = codigo,
            Nome = request.Nome.Trim(),
            UnidadeId = unidade.Id,
            UsuarioId = usuario.Id
        };

        await _colaboradores.AddAsync(colaborador, cancellationToken);
        await _colaboradores.SaveChangesAsync(cancellationToken);

        return await ObterResponseAsync(colaborador.Id, cancellationToken);
    }

    public async Task<ColaboradorResponse> AtualizarAsync(string codigo, UpdateColaboradorRequest request, CancellationToken cancellationToken = default)
    {
        var colaborador = await _colaboradores.Query()
            .FirstOrDefaultAsync(x => x.Codigo == codigo.Trim().ToUpperInvariant(), cancellationToken)
            ?? throw new NotFoundException($"Colaborador '{codigo}' não encontrado.");

        var unidade = await ObterUnidadeParaAssociacaoAsync(request.UnidadeId, cancellationToken);

        colaborador.Nome = request.Nome.Trim();
        colaborador.UnidadeId = unidade.Id;

        _colaboradores.Update(colaborador);
        await _colaboradores.SaveChangesAsync(cancellationToken);

        return await ObterResponseAsync(colaborador.Id, cancellationToken);
    }

    public async Task RemoverAsync(string codigo, CancellationToken cancellationToken = default)
    {
        var colaborador = await _colaboradores.Query()
            .FirstOrDefaultAsync(x => x.Codigo == codigo.Trim().ToUpperInvariant(), cancellationToken)
            ?? throw new NotFoundException($"Colaborador '{codigo}' não encontrado.");

        _colaboradores.Remove(colaborador);
        await _colaboradores.SaveChangesAsync(cancellationToken);
    }

    private async Task<Unidade> ObterUnidadeParaAssociacaoAsync(Guid unidadeId, CancellationToken cancellationToken)
    {
        var unidade = await _unidades.GetByIdAsync(unidadeId, cancellationToken)
            ?? throw new NotFoundException("Unidade não encontrada.");

        if (!unidade.EstaAtiva())
        {
            throw new BusinessRuleException("A unidade selecionada está inativa e não pode receber colaboradores.");
        }

        return unidade;
    }

    private IQueryable<Colaborador> QueryComRelacionamentos()
        => _colaboradores.Query().Include(x => x.Unidade).Include(x => x.Usuario);

    private async Task<ColaboradorResponse> ObterResponseAsync(Guid id, CancellationToken cancellationToken)
    {
        var colaborador = await QueryComRelacionamentos().FirstAsync(x => x.Id == id, cancellationToken);
        return Map(colaborador);
    }

    private static ColaboradorResponse Map(Colaborador colaborador) => new()
    {
        Id = colaborador.Id,
        Codigo = colaborador.Codigo,
        Nome = colaborador.Nome,
        UnidadeId = colaborador.UnidadeId,
        UnidadeCodigo = colaborador.Unidade.Codigo,
        UnidadeNome = colaborador.Unidade.Nome,
        UsuarioId = colaborador.UsuarioId,
        UsuarioLogin = colaborador.Usuario.Login
    };
}

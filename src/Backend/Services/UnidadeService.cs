using Microsoft.EntityFrameworkCore;
using Rte.Api.DTOs.Colaboradores;
using Rte.Api.DTOs.Unidades;
using Rte.Api.Entities;
using Rte.Api.Exceptions;
using Rte.Api.Repositories;

namespace Rte.Api.Services;

public interface IUnidadeService
{
    Task<IReadOnlyList<UnidadeResponse>> ListarAsync(CancellationToken cancellationToken = default);
    Task<UnidadeDetalheResponse> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UnidadeResponse> CriarAsync(CreateUnidadeRequest request, CancellationToken cancellationToken = default);
    Task<UnidadeResponse> AtualizarAsync(Guid id, UpdateUnidadeRequest request, CancellationToken cancellationToken = default);
}

public class UnidadeService : IUnidadeService
{
    private readonly IRepository<Unidade> _unidades;

    public UnidadeService(IRepository<Unidade> unidades)
    {
        _unidades = unidades;
    }

    public async Task<IReadOnlyList<UnidadeResponse>> ListarAsync(CancellationToken cancellationToken = default)
    {
        return await _unidades.Query()
            .Include(x => x.Colaboradores)
            .OrderBy(x => x.Codigo)
            .Select(x => new UnidadeResponse
            {
                Id = x.Id,
                Codigo = x.Codigo,
                Nome = x.Nome,
                Status = x.Status,
                QuantidadeColaboradores = x.Colaboradores.Count
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<UnidadeDetalheResponse> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var unidade = await _unidades.Query()
            .Include(x => x.Colaboradores)
            .ThenInclude(x => x.Usuario)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundException("Unidade não encontrada.");

        return new UnidadeDetalheResponse
        {
            Id = unidade.Id,
            Codigo = unidade.Codigo,
            Nome = unidade.Nome,
            Status = unidade.Status,
            QuantidadeColaboradores = unidade.Colaboradores.Count,
            Colaboradores = unidade.Colaboradores
                .OrderBy(c => c.Codigo)
                .Select(c => new ColaboradorResponse
                {
                    Id = c.Id,
                    Codigo = c.Codigo,
                    Nome = c.Nome,
                    UnidadeId = unidade.Id,
                    UnidadeCodigo = unidade.Codigo,
                    UnidadeNome = unidade.Nome,
                    UsuarioId = c.UsuarioId,
                    UsuarioLogin = c.Usuario.Login
                })
                .ToList()
        };
    }

    public async Task<UnidadeResponse> CriarAsync(CreateUnidadeRequest request, CancellationToken cancellationToken = default)
    {
        var codigo = request.Codigo.Trim().ToUpperInvariant();

        if (await _unidades.Query().AnyAsync(x => x.Codigo == codigo, cancellationToken))
        {
            throw new ConflictException($"Já existe uma unidade com o código '{codigo}'.");
        }

        var unidade = new Unidade
        {
            Codigo = codigo,
            Nome = request.Nome.Trim(),
            Status = request.Status
        };

        await _unidades.AddAsync(unidade, cancellationToken);
        await _unidades.SaveChangesAsync(cancellationToken);

        return Map(unidade, 0);
    }

    public async Task<UnidadeResponse> AtualizarAsync(Guid id, UpdateUnidadeRequest request, CancellationToken cancellationToken = default)
    {
        var unidade = await _unidades.Query()
            .Include(x => x.Colaboradores)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundException("Unidade não encontrada.");

        unidade.Nome = request.Nome.Trim();
        unidade.Status = request.Status;

        _unidades.Update(unidade);
        await _unidades.SaveChangesAsync(cancellationToken);

        return Map(unidade, unidade.Colaboradores.Count);
    }

    private static UnidadeResponse Map(Unidade unidade, int quantidade) => new()
    {
        Id = unidade.Id,
        Codigo = unidade.Codigo,
        Nome = unidade.Nome,
        Status = unidade.Status,
        QuantidadeColaboradores = quantidade
    };
}

using Microsoft.EntityFrameworkCore;
using Rte.Api.Data;
using Rte.Api.Entities;

namespace Rte.Api.Repositories;

public class Repository<T> : IRepository<T> where T : EntityBase
{
    private readonly AppDbContext _context;
    private readonly DbSet<T> _set;

    public Repository(AppDbContext context)
    {
        _context = context;
        _set = context.Set<T>();
    }

    public IQueryable<T> Query() => _set.AsQueryable();

    public Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _set.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        => await _set.AddAsync(entity, cancellationToken);

    public void Update(T entity) => _set.Update(entity);

    public void Remove(T entity) => _set.Remove(entity);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);
}

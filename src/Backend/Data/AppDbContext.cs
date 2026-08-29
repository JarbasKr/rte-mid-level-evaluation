using Microsoft.EntityFrameworkCore;
using Rte.Api.Entities;

namespace Rte.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Unidade> Unidades => Set<Unidade>();
    public DbSet<Colaborador> Colaboradores => Set<Colaborador>();

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var utcNow = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<EntityBase>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = utcNow;
                entry.Entity.UpdatedAt = utcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = utcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("usuarios");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Codigo).HasMaxLength(20).IsRequired();
            entity.Property(x => x.Login).HasMaxLength(80).IsRequired();
            entity.Property(x => x.SenhaHash).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
            entity.HasIndex(x => x.Codigo).IsUnique();
            entity.HasIndex(x => x.Login).IsUnique();
        });

        modelBuilder.Entity<Unidade>(entity =>
        {
            entity.ToTable("unidades");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Codigo).HasMaxLength(20).IsRequired();
            entity.Property(x => x.Nome).HasMaxLength(120).IsRequired();
            entity.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
            entity.HasIndex(x => x.Codigo).IsUnique();
        });

        modelBuilder.Entity<Colaborador>(entity =>
        {
            entity.ToTable("colaboradores");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Codigo).HasMaxLength(20).IsRequired();
            entity.Property(x => x.Nome).HasMaxLength(120).IsRequired();
            entity.HasIndex(x => x.Codigo).IsUnique();
            entity.HasIndex(x => x.UsuarioId).IsUnique();

            entity.HasOne(x => x.Unidade)
                .WithMany(x => x.Colaboradores)
                .HasForeignKey(x => x.UnidadeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Usuario)
                .WithOne(x => x.Colaborador)
                .HasForeignKey<Colaborador>(x => x.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}

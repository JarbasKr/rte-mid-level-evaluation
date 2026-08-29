using Microsoft.EntityFrameworkCore;
using Rte.Api.Authentication;
using Rte.Api.Entities;

namespace Rte.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db, IPasswordHasher passwordHasher, CancellationToken cancellationToken = default)
    {
        if (await db.Usuarios.AnyAsync(cancellationToken))
        {
            return;
        }

        var admin = new Usuario
        {
            Codigo = "ADM001",
            Login = "admin",
            SenhaHash = passwordHasher.Hash("Admin@123"),
            Status = StatusRegistro.Ativo
        };

        var colaboradorUser = new Usuario
        {
            Codigo = "USR001",
            Login = "msilva",
            SenhaHash = passwordHasher.Hash("Colab@123"),
            Status = StatusRegistro.Ativo
        };

        var matriz = new Unidade
        {
            Codigo = "UND001",
            Nome = "Matriz São Paulo",
            Status = StatusRegistro.Ativo
        };

        var unidadeInativa = new Unidade
        {
            Codigo = "UND002",
            Nome = "Filial Inativa (demonstração)",
            Status = StatusRegistro.Inativo
        };

        db.Usuarios.AddRange(admin, colaboradorUser);
        db.Unidades.AddRange(matriz, unidadeInativa);
        await db.SaveChangesAsync(cancellationToken);

        db.Colaboradores.Add(new Colaborador
        {
            Codigo = "COL001",
            Nome = "Maria Silva",
            UnidadeId = matriz.Id,
            UsuarioId = colaboradorUser.Id
        });

        await db.SaveChangesAsync(cancellationToken);
    }
}

namespace TaskFlow.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

/// <summary>
/// Fábrica en tiempo de diseño para TaskFlowDbContext, requerida por las herramientas de EF Core (dotnet ef).
/// Utiliza una cadena de conexión local segura sin credenciales versionadas y con validación TLS habilitada.
/// </summary>
public class TaskFlowDbContextFactory : IDesignTimeDbContextFactory<TaskFlowDbContext>
{
    public TaskFlowDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__TaskFlowDb")
            ?? "Server=(localdb)\\mssqllocaldb;Database=TaskFlowDb;Trusted_Connection=True;Encrypt=True;";

        var optionsBuilder = new DbContextOptionsBuilder<TaskFlowDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new TaskFlowDbContext(optionsBuilder.Options);
    }
}

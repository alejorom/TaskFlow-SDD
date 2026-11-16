namespace TaskFlow.IntegrationTests.Infrastructure;

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Infrastructure.Persistence;
using Xunit;

public class TaskFlowDbContextFactoryTests
{
    [Fact]
    public void CreateDbContext_ReturnsConfiguredTaskFlowDbContext()
    {
        var factory = new TaskFlowDbContextFactory();
        using var context = factory.CreateDbContext([]);

        Assert.NotNull(context);
        Assert.NotNull(context.Tasks);
    }

    [Fact]
    public void CreateDbContext_DefaultConfiguration_DoesNotDisableTlsValidation()
    {
        var envConnection = Environment.GetEnvironmentVariable("ConnectionStrings__TaskFlowDb");

        // Si no se suministra una cadena de conexión explícita por entorno,
        // se verifica que la configuración predeterminada mantenga la validación TLS activa.
        if (string.IsNullOrWhiteSpace(envConnection))
        {
            var factory = new TaskFlowDbContextFactory();
            using var context = factory.CreateDbContext([]);

            var connectionString = context.Database.GetConnectionString();
            Assert.NotNull(connectionString);

            var builder = new SqlConnectionStringBuilder(connectionString);
            Assert.False(builder.TrustServerCertificate, "TrustServerCertificate must be false by default to enforce TLS certificate validation.");
            Assert.True(builder.Encrypt, "Encryption must remain enabled for SQL Server communication.");
        }
        else
        {
            // Si el entorno ya provee una conexión específica, se respeta dicha configuración.
            var factory = new TaskFlowDbContextFactory();
            using var context = factory.CreateDbContext([]);

            var connectionString = context.Database.GetConnectionString();
            Assert.NotNull(connectionString);

            var builder = new SqlConnectionStringBuilder(connectionString);
            Assert.False(string.IsNullOrWhiteSpace(builder.DataSource));
        }
    }
}

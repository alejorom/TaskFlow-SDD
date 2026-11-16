namespace TaskFlow.IntegrationTests.Infrastructure;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskFlow.Infrastructure;
using TaskFlow.Infrastructure.Persistence;
using Xunit;

public class InfrastructureServiceCollectionExtensionsTests
{
    [Fact]
    public void AddInfrastructureServices_WithConnectionStringInConfiguration_RegistersDbContext()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:TaskFlowDb"] = "Server=localhost;Database=TaskFlowDb;Integrated Security=True;Encrypt=True;"
            })
            .Build();

        services.AddInfrastructureServices(configuration);
        using var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();

        var dbContext = scope.ServiceProvider.GetService<TaskFlowDbContext>();
        Assert.NotNull(dbContext);
    }

    [Fact]
    public void AddInfrastructureServices_WithoutConnectionString_ThrowsInvalidOperationException()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        Assert.Throws<InvalidOperationException>(() =>
            services.AddInfrastructureServices(configuration));
    }
}

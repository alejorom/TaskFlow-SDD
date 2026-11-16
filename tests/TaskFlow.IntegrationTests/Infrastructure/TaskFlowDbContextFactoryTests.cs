namespace TaskFlow.IntegrationTests.Infrastructure;

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
}

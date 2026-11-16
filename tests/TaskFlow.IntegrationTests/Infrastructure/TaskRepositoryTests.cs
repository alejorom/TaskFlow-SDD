namespace TaskFlow.IntegrationTests.Infrastructure;

using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Abstractions;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;
using TaskFlow.Infrastructure.Persistence;
using TaskFlow.Infrastructure.Persistence.Repositories;
using Xunit;

public class TaskRepositoryTests
{
    private static DbContextOptions<TaskFlowDbContext> CreateDbContextOptions()
    {
        return new DbContextOptionsBuilder<TaskFlowDbContext>()
            .UseSqlServer("Server=localhost;Database=TaskFlowDb;Integrated Security=True;Encrypt=True;")
            .Options;
    }

    [Fact]
    public void Constructor_WithNullDbContext_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new TaskRepository(null!));
    }

    [Fact]
    public void TaskRepository_ImplementsITaskRepository()
    {
        using var context = new TaskFlowDbContext(CreateDbContextOptions());
        var repository = new TaskRepository(context);

        Assert.IsAssignableFrom<ITaskRepository>(repository);
    }

    [Fact]
    public async Task AddAsync_WithNullTask_ThrowsArgumentNullException()
    {
        using var context = new TaskFlowDbContext(CreateDbContextOptions());
        var repository = new TaskRepository(context);

        await Assert.ThrowsAsync<ArgumentNullException>(() => repository.AddAsync(null!));
    }

    [Fact]
    public async Task AddAsync_WithValidTask_AddsEntityToChangeTrackerAsAdded()
    {
        using var context = new TaskFlowDbContext(CreateDbContextOptions());
        var repository = new TaskRepository(context);

        var task = TaskItem.Create(
            title: "Test Task",
            description: "Test Description",
            dueDate: DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            priority: TaskPriority.High);

        await repository.AddAsync(task);

        var entry = context.Entry(task);
        Assert.NotNull(entry);
        Assert.Equal(EntityState.Added, entry.State);
    }
}

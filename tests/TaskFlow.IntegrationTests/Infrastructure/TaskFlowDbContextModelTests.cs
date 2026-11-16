namespace TaskFlow.IntegrationTests.Infrastructure;

using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;
using TaskFlow.Infrastructure.Persistence;
using Xunit;

public class TaskFlowDbContextModelTests
{
    [Fact]
    public void ModelConfiguration_ConfiguresTaskItemCorrectly()
    {
        var options = new DbContextOptionsBuilder<TaskFlowDbContext>()
            .UseSqlServer("Server=localhost;Database=TaskFlowDb;Integrated Security=True;Encrypt=True;")
            .Options;

        using var context = new TaskFlowDbContext(options);
        var entityType = context.Model.FindEntityType(typeof(TaskItem));

        Assert.NotNull(entityType);
        Assert.Equal("Tasks", entityType.GetTableName());

        var idProperty = entityType.FindProperty(nameof(TaskItem.Id));
        Assert.NotNull(idProperty);
        Assert.True(idProperty.IsPrimaryKey());

        var titleProperty = entityType.FindProperty(nameof(TaskItem.Title));
        Assert.NotNull(titleProperty);
        Assert.False(titleProperty.IsNullable);
        Assert.Equal(200, titleProperty.GetMaxLength());

        var descriptionProperty = entityType.FindProperty(nameof(TaskItem.Description));
        Assert.NotNull(descriptionProperty);
        Assert.True(descriptionProperty.IsNullable);
        Assert.Equal(2000, descriptionProperty.GetMaxLength());

        var dueDateProperty = entityType.FindProperty(nameof(TaskItem.DueDate));
        Assert.NotNull(dueDateProperty);
        Assert.True(dueDateProperty.IsNullable);
        Assert.Equal("date", dueDateProperty.GetColumnType());

        var priorityProperty = entityType.FindProperty(nameof(TaskItem.Priority));
        Assert.NotNull(priorityProperty);
        Assert.False(priorityProperty.IsNullable);
        Assert.Equal(20, priorityProperty.GetMaxLength());
        Assert.Equal("nvarchar(20)", priorityProperty.GetColumnType());
        Assert.Equal(typeof(string), priorityProperty.GetProviderClrType());

        var statusProperty = entityType.FindProperty(nameof(TaskItem.Status));
        Assert.NotNull(statusProperty);
        Assert.False(statusProperty.IsNullable);
        Assert.Equal(20, statusProperty.GetMaxLength());
        Assert.Equal("nvarchar(20)", statusProperty.GetColumnType());
        Assert.Equal(typeof(string), statusProperty.GetProviderClrType());

        var createdAtProperty = entityType.FindProperty(nameof(TaskItem.CreatedAt));
        Assert.NotNull(createdAtProperty);
        Assert.False(createdAtProperty.IsNullable);
        Assert.Equal("datetimeoffset", createdAtProperty.GetColumnType());
    }
}

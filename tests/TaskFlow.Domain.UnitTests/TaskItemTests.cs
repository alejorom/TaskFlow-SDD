namespace TaskFlow.Domain.UnitTests;

using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;
using TaskFlow.Domain.Exceptions;

public class TaskItemTests
{
    private readonly DateOnly _today = new(2026, 11, 12);

    [Fact]
    public void Create_WithOnlyRequiredTitle_SetsDefaultsAndGeneratesUniqueId()
    {
        // Arrange
        const string title = "Planificar sprint";

        // Act
        var task = TaskItem.Create(title, todayUtc: _today);

        // Assert
        Assert.NotEqual(Guid.Empty, task.Id);
        Assert.Equal(title, task.Title);
        Assert.Null(task.Description);
        Assert.Null(task.DueDate);
        Assert.Equal(TaskPriority.Medium, task.Priority);
        Assert.Equal(TaskStatus.Pending, task.Status);
        Assert.True(task.CreatedAt <= DateTimeOffset.UtcNow);
    }

    [Fact]
    public void Create_WithAllValidFields_InitializesPropertiesCorrectly()
    {
        // Arrange
        const string title = "Completar reporte";
        const string description = "Reporte mensual detallado con métricas de desempeño";
        var dueDate = _today.AddDays(7);
        const TaskPriority priority = TaskPriority.High;
        var createdAt = new DateTimeOffset(2026, 11, 12, 10, 0, 0, TimeSpan.Zero);

        // Act
        var task = TaskItem.Create(title, description, dueDate, priority, createdAt, _today);

        // Assert
        Assert.NotEqual(Guid.Empty, task.Id);
        Assert.Equal(title, task.Title);
        Assert.Equal(description, task.Description);
        Assert.Equal(dueDate, task.DueDate);
        Assert.Equal(TaskPriority.High, task.Priority);
        Assert.Equal(TaskStatus.Pending, task.Status);
        Assert.Equal(createdAt, task.CreatedAt);
    }

    [Theory]
    [InlineData(TaskPriority.Low)]
    [InlineData(TaskPriority.Medium)]
    [InlineData(TaskPriority.High)]
    public void Create_WithExplicitPriority_SetsPriorityAndPendingStatus(TaskPriority priority)
    {
        // Act
        var task = TaskItem.Create("Tarea con prioridad", priority: priority, todayUtc: _today);

        // Assert
        Assert.Equal(priority, task.Priority);
        Assert.Equal(TaskStatus.Pending, task.Status);
    }

    [Fact]
    public void Create_WithDueDateToday_Succeeds()
    {
        // Act
        var task = TaskItem.Create("Tarea para hoy", dueDate: _today, todayUtc: _today);

        // Assert
        Assert.Equal(_today, task.DueDate);
    }

    [Fact]
    public void Create_WithDueDateInFuture_Succeeds()
    {
        // Arrange
        var futureDate = _today.AddDays(30);

        // Act
        var task = TaskItem.Create("Tarea futura", dueDate: futureDate, todayUtc: _today);

        // Assert
        Assert.Equal(futureDate, task.DueDate);
    }

    [Fact]
    public void Create_WithSurroundingWhitespaceInTitle_TrimsTitle()
    {
        // Act
        var task = TaskItem.Create("   Título con espacios   ", todayUtc: _today);

        // Assert
        Assert.Equal("Título con espacios", task.Title);
    }

    [Fact]
    public void Create_WithWhitespaceOnlyDescription_SetsDescriptionToNull()
    {
        // Act
        var task = TaskItem.Create("Título válido", description: "   ", todayUtc: _today);

        // Assert
        Assert.Null(task.Description);
    }

    [Fact]
    public void Create_WithMaxAllowedTitleLength_Succeeds()
    {
        // Arrange
        var maxTitle = new string('A', 200);

        // Act
        var task = TaskItem.Create(maxTitle, todayUtc: _today);

        // Assert
        Assert.Equal(maxTitle, task.Title);
    }

    [Fact]
    public void Create_WithMaxAllowedDescriptionLength_Succeeds()
    {
        // Arrange
        var maxDescription = new string('B', 2000);

        // Act
        var task = TaskItem.Create("Título válido", description: maxDescription, todayUtc: _today);

        // Assert
        Assert.Equal(maxDescription, task.Description);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithNullOrWhitespaceTitle_ThrowsDomainValidationException(string? invalidTitle)
    {
        // Act & Assert
        var ex = Assert.Throws<DomainValidationException>(() =>
            TaskItem.Create(invalidTitle!, todayUtc: _today));

        Assert.Equal("Title is required.", ex.Message);
    }

    [Fact]
    public void Create_WithTitleExceeding200Characters_ThrowsDomainValidationException()
    {
        // Arrange
        var longTitle = new string('A', 201);

        // Act & Assert
        var ex = Assert.Throws<DomainValidationException>(() =>
            TaskItem.Create(longTitle, todayUtc: _today));

        Assert.Equal("Title cannot exceed 200 characters.", ex.Message);
    }

    [Fact]
    public void Create_WithDescriptionExceeding2000Characters_ThrowsDomainValidationException()
    {
        // Arrange
        var longDescription = new string('D', 2001);

        // Act & Assert
        var ex = Assert.Throws<DomainValidationException>(() =>
            TaskItem.Create("Título válido", description: longDescription, todayUtc: _today));

        Assert.Equal("Description cannot exceed 2000 characters.", ex.Message);
    }

    [Fact]
    public void Create_WithDueDateInThePast_ThrowsDomainValidationException()
    {
        // Arrange
        var pastDate = _today.AddDays(-1);

        // Act & Assert
        var ex = Assert.Throws<DomainValidationException>(() =>
            TaskItem.Create("Tarea atrasada", dueDate: pastDate, todayUtc: _today));

        Assert.Equal("The due date cannot be in the past.", ex.Message);
    }

    [Fact]
    public void Create_WithInvalidPriority_ThrowsDomainValidationException()
    {
        // Act & Assert
        var ex = Assert.Throws<DomainValidationException>(() =>
            TaskItem.Create("Tarea inválida", priority: (TaskPriority)999, todayUtc: _today));

        Assert.Equal("Invalid priority value.", ex.Message);
    }

    [Fact]
    public void Constructor_DirectInvocation_AppliesSameRulesAsFactoryMethod()
    {
        // Act
        var task = new TaskItem("Creación directa", todayUtc: _today);

        // Assert
        Assert.NotEqual(Guid.Empty, task.Id);
        Assert.Equal("Creación directa", task.Title);
        Assert.Equal(TaskPriority.Medium, task.Priority);
        Assert.Equal(TaskStatus.Pending, task.Status);
    }
}


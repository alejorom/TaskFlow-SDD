namespace TaskFlow.Application.UnitTests.Features.Tasks.CreateTask;

using Microsoft.Extensions.Time.Testing;
using TaskFlow.Application.Abstractions;
using TaskFlow.Application.Features.Tasks.CreateTask;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;
using TaskFlow.Domain.Exceptions;
using TaskStatus = TaskFlow.Domain.Enums.TaskStatus;

/// <summary>
/// Doble de prueba (fake) de ITaskRepository en memoria.
/// Registra las llamadas para verificar orquestación y ausencia de persistencia ante fallos.
/// </summary>
internal sealed class FakeTaskRepository : ITaskRepository
{
    private readonly List<TaskItem> _store = [];

    public IReadOnlyList<TaskItem> StoredTasks => _store.AsReadOnly();
    public int AddAsyncCallCount { get; private set; }
    public int SaveChangesAsyncCallCount { get; private set; }

    public Task AddAsync(TaskItem task, CancellationToken cancellationToken = default)
    {
        AddAsyncCallCount++;
        _store.Add(task);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SaveChangesAsyncCallCount++;
        return Task.CompletedTask;
    }
}

public class CreateTaskCommandHandlerTests
{
    // Fecha fija para pruebas deterministas (evita flakiness por cambio de día).
    private static readonly DateTimeOffset FixedUtcNow = new(2026, 11, 12, 10, 0, 0, TimeSpan.Zero);
    private static readonly DateOnly TodayUtc = DateOnly.FromDateTime(FixedUtcNow.UtcDateTime);

    private static (CreateTaskCommandHandler Handler, FakeTaskRepository Repository, FakeTimeProvider TimeProvider)
        BuildSut()
    {
        var fakeTime = new FakeTimeProvider(FixedUtcNow);
        var repo = new FakeTaskRepository();
        var handler = new CreateTaskCommandHandler(repo, fakeTime);
        return (handler, repo, fakeTime);
    }

    // ── Escenario: creación exitosa con campos obligatorios ──────────────────────────

    [Fact]
    public async Task HandleAsync_WithValidTitle_ReturnsCreatedTask()
    {
        // Arrange
        var (handler, _, _) = BuildSut();
        var command = new CreateTaskCommand("Planificar sprint", null, null);

        // Act
        var response = await handler.HandleAsync(command);

        // Assert
        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal("Planificar sprint", response.Title);
        Assert.Null(response.Description);
        Assert.Null(response.DueDate);
        Assert.Equal(TaskPriority.Medium, response.Priority);
        Assert.Equal(TaskStatus.Pending, response.Status);
        Assert.Equal(FixedUtcNow, response.CreatedAt);
    }

    // ── Escenario: asignación automática de estado Pending y prioridad Medium ───────

    [Fact]
    public async Task HandleAsync_WithoutPriority_AssignsMediumPriorityAndPendingStatus()
    {
        // Arrange
        var (handler, _, _) = BuildSut();
        var command = new CreateTaskCommand("Tarea sin prioridad", null, null);

        // Act
        var response = await handler.HandleAsync(command);

        // Assert
        Assert.Equal(TaskPriority.Medium, response.Priority);
        Assert.Equal(TaskStatus.Pending, response.Status);
    }

    [Theory]
    [InlineData(TaskPriority.Low)]
    [InlineData(TaskPriority.Medium)]
    [InlineData(TaskPriority.High)]
    public async Task HandleAsync_WithExplicitPriority_SetsPriorityAndPendingStatus(TaskPriority priority)
    {
        // Arrange
        var (handler, _, _) = BuildSut();
        var command = new CreateTaskCommand("Tarea con prioridad", null, null, priority);

        // Act
        var response = await handler.HandleAsync(command);

        // Assert
        Assert.Equal(priority, response.Priority);
        Assert.Equal(TaskStatus.Pending, response.Status);
    }

    // ── Escenario: fecha de referencia inyectada desde FakeTimeProvider ──────────────

    [Fact]
    public async Task HandleAsync_UsesTimeProviderForCreatedAt()
    {
        // Arrange
        var (handler, _, _) = BuildSut();
        var command = new CreateTaskCommand("Tarea temporal", null, null);

        // Act
        var response = await handler.HandleAsync(command);

        // Assert: CreatedAt debe coincidir con el instante fijo del FakeTimeProvider.
        Assert.Equal(FixedUtcNow, response.CreatedAt);
    }

    [Fact]
    public async Task HandleAsync_WithDueDateToday_Succeeds()
    {
        // Arrange
        var (handler, _, _) = BuildSut();
        var command = new CreateTaskCommand("Tarea para hoy", null, TodayUtc);

        // Act
        var response = await handler.HandleAsync(command);

        // Assert
        Assert.Equal(TodayUtc, response.DueDate);
    }

    [Fact]
    public async Task HandleAsync_WithDueDateInFuture_Succeeds()
    {
        // Arrange
        var (handler, _, _) = BuildSut();
        var futureDate = TodayUtc.AddDays(30);
        var command = new CreateTaskCommand("Tarea futura", null, futureDate);

        // Act
        var response = await handler.HandleAsync(command);

        // Assert
        Assert.Equal(futureDate, response.DueDate);
    }

    // ── Escenario: orquestación — AddAsync y SaveChangesAsync llamados en orden ─────

    [Fact]
    public async Task HandleAsync_OnSuccess_CallsAddAsyncThenSaveChangesAsync()
    {
        // Arrange
        var (handler, repo, _) = BuildSut();
        var command = new CreateTaskCommand("Tarea orquestada", null, null);

        // Act
        await handler.HandleAsync(command);

        // Assert: ambas operaciones de persistencia se invocan exactamente una vez.
        Assert.Equal(1, repo.AddAsyncCallCount);
        Assert.Equal(1, repo.SaveChangesAsyncCallCount);
    }

    [Fact]
    public async Task HandleAsync_OnSuccess_PersistsTaskInRepository()
    {
        // Arrange
        var (handler, repo, _) = BuildSut();
        var command = new CreateTaskCommand("Tarea almacenada", null, null);

        // Act
        var response = await handler.HandleAsync(command);

        // Assert
        var stored = Assert.Single(repo.StoredTasks);
        Assert.Equal(response.Id, stored.Id);
        Assert.Equal("Tarea almacenada", stored.Title);
    }

    // ── Escenario: manejo de excepciones de dominio y ausencia de persistencia ──────

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task HandleAsync_WithNullOrWhitespaceTitle_ThrowsDomainValidationException_NoPersistence(string? invalidTitle)
    {
        // Arrange
        var (handler, repo, _) = BuildSut();
        var command = new CreateTaskCommand(invalidTitle!, null, null);

        // Act & Assert
        await Assert.ThrowsAsync<DomainValidationException>(() => handler.HandleAsync(command));

        // No persistencia ante fallo de validación (DomainValidationException lanzada antes de AddAsync).
        Assert.Equal(0, repo.AddAsyncCallCount);
        Assert.Equal(0, repo.SaveChangesAsyncCallCount);
        Assert.Empty(repo.StoredTasks);
    }

    [Fact]
    public async Task HandleAsync_WithTitleExceeding200Chars_ThrowsDomainValidationException_NoPersistence()
    {
        // Arrange
        var (handler, repo, _) = BuildSut();
        var command = new CreateTaskCommand(new string('A', 201), null, null);

        // Act & Assert
        await Assert.ThrowsAsync<DomainValidationException>(() => handler.HandleAsync(command));
        Assert.Equal(0, repo.AddAsyncCallCount);
        Assert.Equal(0, repo.SaveChangesAsyncCallCount);
    }

    [Fact]
    public async Task HandleAsync_WithDescriptionExceeding2000Chars_ThrowsDomainValidationException_NoPersistence()
    {
        // Arrange
        var (handler, repo, _) = BuildSut();
        var command = new CreateTaskCommand("Título válido", new string('D', 2001), null);

        // Act & Assert
        await Assert.ThrowsAsync<DomainValidationException>(() => handler.HandleAsync(command));
        Assert.Equal(0, repo.AddAsyncCallCount);
        Assert.Equal(0, repo.SaveChangesAsyncCallCount);
    }

    [Fact]
    public async Task HandleAsync_WithPastDueDate_ThrowsDomainValidationException_NoPersistence()
    {
        // Arrange
        var (handler, repo, _) = BuildSut();
        var pastDate = TodayUtc.AddDays(-1);
        var command = new CreateTaskCommand("Tarea atrasada", null, pastDate);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<DomainValidationException>(() => handler.HandleAsync(command));
        Assert.Equal("The due date cannot be in the past.", ex.Message);
        Assert.Equal(0, repo.AddAsyncCallCount);
        Assert.Equal(0, repo.SaveChangesAsyncCallCount);
        Assert.Empty(repo.StoredTasks);
    }

    [Fact]
    public async Task HandleAsync_WithInvalidPriority_ThrowsDomainValidationException_NoPersistence()
    {
        // Arrange
        var (handler, repo, _) = BuildSut();
        var command = new CreateTaskCommand("Tarea inválida", null, null, (TaskPriority)999);

        // Act & Assert
        await Assert.ThrowsAsync<DomainValidationException>(() => handler.HandleAsync(command));
        Assert.Equal(0, repo.AddAsyncCallCount);
        Assert.Equal(0, repo.SaveChangesAsyncCallCount);
        Assert.Empty(repo.StoredTasks);
    }

    // ── Escenario: descripción con espacios en blanco se normaliza a null ─────────

    [Fact]
    public async Task HandleAsync_WithWhitespaceDescription_ReturnsNullDescription()
    {
        // Arrange
        var (handler, _, _) = BuildSut();
        var command = new CreateTaskCommand("Título válido", "   ", null);

        // Act
        var response = await handler.HandleAsync(command);

        // Assert: el dominio normaliza la descripción a null.
        Assert.Null(response.Description);
    }
}


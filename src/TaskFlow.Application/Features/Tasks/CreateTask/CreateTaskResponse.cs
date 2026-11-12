namespace TaskFlow.Application.Features.Tasks.CreateTask;

using TaskFlow.Domain.Enums;

/// <summary>
/// DTO inmutable devuelto al cliente tras la creación exitosa de una tarea.
/// </summary>
public sealed record CreateTaskResponse(
    Guid Id,
    string Title,
    string? Description,
    DateOnly? DueDate,
    TaskPriority Priority,
    TaskStatus Status,
    DateTimeOffset CreatedAt);


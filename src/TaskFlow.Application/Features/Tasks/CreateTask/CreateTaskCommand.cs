namespace TaskFlow.Application.Features.Tasks.CreateTask;

using TaskFlow.Domain.Enums;

/// <summary>
/// Datos de entrada para el caso de uso CreateTask.
/// Registro inmutable que transporta la intención del cliente hacia el manejador.
/// </summary>
public sealed record CreateTaskCommand(
    string Title,
    string? Description,
    DateOnly? DueDate,
    TaskPriority Priority = TaskPriority.Medium);


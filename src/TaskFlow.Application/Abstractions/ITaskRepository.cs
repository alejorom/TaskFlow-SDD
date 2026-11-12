namespace TaskFlow.Application.Abstractions;

using TaskFlow.Domain.Entities;

/// <summary>
/// Abstracción de persistencia para el agregado de tareas.
/// Implementada en la capa de Infrastructure para mantener Application independiente de EF Core.
/// </summary>
public interface ITaskRepository
{
    /// <summary>Agrega una nueva tarea al contexto de persistencia (sin persistir aún).</summary>
    Task AddAsync(TaskItem task, CancellationToken cancellationToken = default);

    /// <summary>Persiste los cambios pendientes de forma atómica.</summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}


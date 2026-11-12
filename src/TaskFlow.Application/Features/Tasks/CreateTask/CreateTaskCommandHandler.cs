namespace TaskFlow.Application.Features.Tasks.CreateTask;

using TaskFlow.Application.Abstractions;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;

/// <summary>
/// Orquesta el caso de uso CreateTask:
/// 1. Obtiene la fecha de referencia UTC desde TimeProvider.
/// 2. Crea y valida la entidad TaskItem (las invariantes se disparan en el constructor).
/// 3. Persiste la tarea de forma atómica: AddAsync + SaveChangesAsync.
///
/// No utiliza MediatR ni ningún intermediario de despacho dinámico (ADR-005, ADR-006).
/// Los fallos de validación de dominio se propagan como DomainValidationException
/// sin llegar a persistencia; la capa API los transforma en HTTP 400.
/// </summary>
public sealed class CreateTaskCommandHandler
{
    private readonly ITaskRepository _repository;
    private readonly TimeProvider _timeProvider;

    public CreateTaskCommandHandler(ITaskRepository repository, TimeProvider timeProvider)
    {
        _repository = repository;
        _timeProvider = timeProvider;
    }

    public async Task<CreateTaskResponse> HandleAsync(
        CreateTaskCommand command,
        CancellationToken cancellationToken = default)
    {
        // Fecha de referencia UTC para validar DueDate (DateOnly, sin hora ni zona horaria).
        DateTimeOffset nowUtc = _timeProvider.GetUtcNow();
        DateOnly todayUtc = DateOnly.FromDateTime(nowUtc.UtcDateTime);

        // La entidad valida todas las invariantes; lanza DomainValidationException si falla.
        var task = TaskItem.Create(
            title: command.Title,
            description: command.Description,
            dueDate: command.DueDate,
            priority: command.Priority,
            createdAt: nowUtc,
            todayUtc: todayUtc);

        // Persistencia atómica: si AddAsync o SaveChangesAsync fallan, no queda registro parcial.
        await _repository.AddAsync(task, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return new CreateTaskResponse(
            Id: task.Id,
            Title: task.Title,
            Description: task.Description,
            DueDate: task.DueDate,
            Priority: task.Priority,
            Status: task.Status,
            CreatedAt: task.CreatedAt);
    }
}


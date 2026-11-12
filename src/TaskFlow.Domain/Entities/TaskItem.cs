namespace TaskFlow.Domain.Entities;

using TaskFlow.Domain.Enums;
using TaskFlow.Domain.Exceptions;
using TaskStatus = TaskFlow.Domain.Enums.TaskStatus;

public class TaskItem
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public DateOnly? DueDate { get; private set; }
    public TaskPriority Priority { get; private set; }
    public TaskStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

#pragma warning disable CS8618
    private TaskItem()
    {
    }
#pragma warning restore CS8618

    public TaskItem(
        string title,
        string? description = null,
        DateOnly? dueDate = null,
        TaskPriority priority = TaskPriority.Medium,
        DateTimeOffset? createdAt = null,
        DateOnly? todayUtc = null)
    {
        ValidateAndInitialize(title, description, dueDate, priority, createdAt, todayUtc);
    }

    public static TaskItem Create(
        string title,
        string? description = null,
        DateOnly? dueDate = null,
        TaskPriority priority = TaskPriority.Medium,
        DateTimeOffset? createdAt = null,
        DateOnly? todayUtc = null)
    {
        return new TaskItem(title, description, dueDate, priority, createdAt, todayUtc);
    }

    private void ValidateAndInitialize(
        string title,
        string? description,
        DateOnly? dueDate,
        TaskPriority priority,
        DateTimeOffset? createdAt,
        DateOnly? todayUtc)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainValidationException("Title is required.");
        }

        string trimmedTitle = title.Trim();
        if (trimmedTitle.Length > 200)
        {
            throw new DomainValidationException("Title cannot exceed 200 characters.");
        }

        if (description != null && description.Length > 2000)
        {
            throw new DomainValidationException("Description cannot exceed 2000 characters.");
        }

        if (!Enum.IsDefined(typeof(TaskPriority), priority))
        {
            throw new DomainValidationException("Invalid priority value.");
        }

        DateTimeOffset creationTime = createdAt ?? DateTimeOffset.UtcNow;
        DateOnly referenceDate = todayUtc ?? DateOnly.FromDateTime(creationTime.UtcDateTime);

        if (dueDate.HasValue && dueDate.Value < referenceDate)
        {
            throw new DomainValidationException("The due date cannot be in the past.");
        }

        Id = Guid.NewGuid();
        Title = trimmedTitle;
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        DueDate = dueDate;
        Priority = priority;
        Status = TaskStatus.Pending;
        CreatedAt = creationTime;
    }
}


namespace TaskFlow.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Enums;

public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.ToTable("Tasks");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .ValueGeneratedNever();

        builder.Property(t => t.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Description)
            .HasMaxLength(2000)
            .IsRequired(false);

        builder.Property(t => t.DueDate)
            .HasColumnType("date")
            .IsRequired(false);

        builder.Property(t => t.Priority)
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasColumnType("nvarchar(20)")
            .IsRequired();

        builder.Property(t => t.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasColumnType("nvarchar(20)")
            .IsRequired();

        builder.Property(t => t.CreatedAt)
            .HasColumnType("datetimeoffset")
            .IsRequired();
    }
}

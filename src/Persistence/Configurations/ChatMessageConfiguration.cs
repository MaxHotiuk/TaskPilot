using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
{
    public void Configure(EntityTypeBuilder<ChatMessage> builder)
    {
        builder.ToTable("ChatMessages");
        builder.HasKey(message => message.Id);

        builder.Property(message => message.Content)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(message => message.MessageType)
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue("Text");

        builder.Property(message => message.HasAttachments)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(message => message.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(message => message.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(message => message.Sender)
            .WithMany(user => user.ChatMessages)
            .HasForeignKey(message => message.SenderId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(message => message.Task)
            .WithMany()
            .HasForeignKey(message => message.TaskId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(message => message.Assignee)
            .WithMany()
            .HasForeignKey(message => message.AssigneeId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(message => message.ChatId);
        builder.HasIndex(message => message.SenderId);
        builder.HasIndex(message => message.TaskId);
        builder.HasIndex(message => message.AssigneeId);
    }
}

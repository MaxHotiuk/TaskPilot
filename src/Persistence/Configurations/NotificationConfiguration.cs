using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");
        builder.HasKey(n => n.Id);
        builder.Property(n => n.Text).IsRequired();
        builder.Property(n => n.Type)
            .IsRequired()
            .HasConversion<int>()
            .HasColumnType("int");
        builder.Property(n => n.IsRead).IsRequired();
        builder.HasOne(n => n.User)
            .WithMany(u => u.Notifications)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(n => n.Board)
            .WithMany()
            .HasForeignKey(n => n.BoardId)
            .OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(n => n.Task)
            .WithMany()
            .HasForeignKey(n => n.TaskId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

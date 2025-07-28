using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class MeetingConfiguration : IEntityTypeConfiguration<Meeting>
{
    public void Configure(EntityTypeBuilder<Meeting> builder)
    {
        builder.ToTable("Meetings");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(m => m.Link)
            .HasMaxLength(500);

        builder.Property(m => m.Description)
            .HasMaxLength(int.MaxValue);

        builder.Property(m => m.Status)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Scheduled");

        builder.Property(m => m.CreatedAt)
            .IsRequired();
        builder.Property(m => m.UpdatedAt)
            .IsRequired();

        builder.HasOne(m => m.Board)
            .WithMany(b => b.Meetings)
            .HasForeignKey(m => m.BoardId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.Creator)
            .WithMany()
            .HasForeignKey(m => m.CreatedBy)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(m => m.BoardId);
        builder.HasIndex(m => m.ScheduledAt);
        builder.HasIndex(m => m.Status);
        builder.HasIndex(m => m.CreatedBy);
    }
}

using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.Configurations;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("Tags");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .UseIdentityColumn();

        builder.Property(t => t.BoardId)
            .IsRequired();

        builder.Property(t => t.Name)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(t => t.Color)
            .HasMaxLength(20);

        builder.Property(t => t.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(t => t.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        // Unique constraint on BoardId + Name
        builder.HasIndex(t => new { t.BoardId, t.Name })
            .IsUnique()
            .HasDatabaseName("UQ_Tags_Board_Name");

        // Index on BoardId
        builder.HasIndex(t => t.BoardId)
            .HasDatabaseName("IX_Tags_BoardId");

        // Relationships
        builder.HasOne(t => t.Board)
            .WithMany(b => b.Tags)
            .HasForeignKey(t => t.BoardId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.Tasks)
            .WithOne(task => task.Tag)
            .HasForeignKey(task => task.TagId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}

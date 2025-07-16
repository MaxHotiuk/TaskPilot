using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class ArchivalJobConfiguration : IEntityTypeConfiguration<ArchivalJob>
{
    public void Configure(EntityTypeBuilder<ArchivalJob> builder)
    {
        builder.ToTable("ArchivalJobs");
        
        builder.HasKey(a => a.Id);
        
        builder.Property(a => a.Status)
            .HasConversion<string>()
            .IsRequired();
        
        builder.Property(a => a.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
        
        builder.Property(a => a.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
        
        builder.Property(a => a.BoardId)
            .IsRequired();
        
        // Relationships
        builder.HasOne(a => a.Board)
            .WithMany(b => b.ArchivalJobs)
            .HasForeignKey(a => a.BoardId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

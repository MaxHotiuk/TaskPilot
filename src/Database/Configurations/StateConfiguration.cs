using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database.Configurations;

public class StateConfiguration : IEntityTypeConfiguration<State>
{
    public void Configure(EntityTypeBuilder<State> builder)
    {
        builder.ToTable("States");
        
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.Id)
            .UseIdentityColumn();
            
        builder.Property(s => s.BoardId)
            .IsRequired();
            
        builder.Property(s => s.Name)
            .HasMaxLength(50)
            .IsRequired();
            
        builder.Property(s => s.Order)
            .IsRequired();
            
        builder.Property(s => s.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
            
        builder.Property(s => s.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
            
        // Unique constraints
        builder.HasIndex(s => new { s.BoardId, s.Name })
            .IsUnique()
            .HasDatabaseName("UQ_States_Board_Name");
            
        builder.HasIndex(s => new { s.BoardId, s.Order })
            .IsUnique()
            .HasDatabaseName("UQ_States_Board_Order");
            
        // Relationships
        builder.HasOne(s => s.Board)
            .WithMany(b => b.States)
            .HasForeignKey(s => s.BoardId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(s => s.Tasks)
            .WithOne(t => t.State)
            .HasForeignKey(t => t.StateId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}

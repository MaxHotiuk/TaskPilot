using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        
        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.EntraId)
            .HasMaxLength(100)
            .IsRequired();
            
        builder.Property(u => u.Username)
            .HasMaxLength(100)
            .IsRequired();
            
        builder.Property(u => u.Email)
            .HasMaxLength(150)
            .IsRequired();
            
        builder.Property(u => u.Role)
            .HasMaxLength(50)
            .IsRequired();
            
        builder.Property(u => u.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
            
        builder.Property(u => u.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(u => u.GoogleAccessToken)
            .HasMaxLength(2048);

        builder.Property(u => u.GoogleRefreshToken)
            .HasMaxLength(512);

        builder.Property(u => u.GoogleTokenExpiry);

        builder.Property(u => u.IsGoogleCalendarConnected)
            .IsRequired()
            .HasDefaultValue(false);

        // Indexes
        builder.HasIndex(u => u.EntraId)
            .IsUnique();
            
        builder.HasIndex(u => u.Email)
            .IsUnique();
            
        // Relationships
        builder.HasMany(u => u.OwnedBoards)
            .WithOne(b => b.Owner)
            .HasForeignKey(b => b.OwnerId)
            .OnDelete(DeleteBehavior.NoAction);
            
        builder.HasMany(u => u.AssignedTasks)
            .WithOne(t => t.Assignee)
            .HasForeignKey(t => t.AssigneeId)
            .OnDelete(DeleteBehavior.SetNull);
            
        builder.HasMany(u => u.Comments)
            .WithOne(c => c.Author)
            .HasForeignKey(c => c.AuthorId)
            .OnDelete(DeleteBehavior.NoAction);
            
        builder.HasMany(u => u.BoardMemberships)
            .WithOne(bm => bm.User)
            .HasForeignKey(bm => bm.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

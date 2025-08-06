using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.Configurations;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("UserProfiles");
        
        builder.HasKey(up => up.Id);
        
        builder.Property(up => up.UserId)
            .IsRequired();
            
        builder.Property(up => up.Bio)
            .HasMaxLength(500);
            
        builder.Property(up => up.JobTitle)
            .HasMaxLength(100);
            
        builder.Property(up => up.Department)
            .HasMaxLength(100);
            
        builder.Property(up => up.Location)
            .HasMaxLength(100);
            
        builder.Property(up => up.PhoneNumber)
            .HasMaxLength(20);
            
        builder.Property(up => up.AddToBoardAutomatically)
            .IsRequired()
            .HasDefaultValue(false);
            
        builder.Property(up => up.ShowEmail)
            .IsRequired()
            .HasDefaultValue(false);
            
        builder.Property(up => up.ShowPhoneNumber)
            .IsRequired()
            .HasDefaultValue(false);
            
        builder.Property(up => up.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
            
        builder.Property(up => up.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
            
        // Indexes
        builder.HasIndex(up => up.UserId)
            .IsUnique();
            
        builder.HasIndex(up => up.Department);
        
        builder.HasIndex(up => up.Location);
        
        builder.HasIndex(up => up.CreatedAt);
        
        builder.HasIndex(up => up.UpdatedAt);
            
        // Relationships
        builder.HasOne<User>()
            .WithOne()
            .HasForeignKey<UserProfile>(up => up.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

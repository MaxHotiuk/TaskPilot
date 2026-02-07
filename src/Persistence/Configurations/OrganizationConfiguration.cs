using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.Configurations;

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ToTable("Organizations");
        
        builder.HasKey(o => o.Id);
        
        builder.Property(o => o.Domain)
            .HasMaxLength(255)
            .IsRequired();
            
        builder.Property(o => o.Name)
            .HasMaxLength(255)
            .IsRequired();
            
        builder.Property(o => o.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
            
        builder.Property(o => o.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
            
        // Indexes
        builder.HasIndex(o => o.Domain)
            .IsUnique();
            
        // Relationships
        builder.HasMany(o => o.Members)
            .WithOne(om => om.Organization)
            .HasForeignKey(om => om.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

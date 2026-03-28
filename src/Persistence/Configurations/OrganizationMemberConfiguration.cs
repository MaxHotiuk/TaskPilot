using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.Configurations;

public class OrganizationMemberConfiguration : IEntityTypeConfiguration<OrganizationMember>
{
    public void Configure(EntityTypeBuilder<OrganizationMember> builder)
    {
        builder.ToTable("OrganizationMembers");
        
        // Composite primary key
        builder.HasKey(om => new { om.OrganizationId, om.UserId });
        
        builder.Property(om => om.IsInvited)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(om => om.Role)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(om => om.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
            
        builder.Property(om => om.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");
            
        // Relationships
        builder.HasOne(om => om.Organization)
            .WithMany(o => o.Members)
            .HasForeignKey(om => om.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(om => om.User)
            .WithMany(u => u.OrganizationMemberships)
            .HasForeignKey(om => om.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

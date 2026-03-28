using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.Configurations;

public class OrganizationManagerRequestConfiguration : IEntityTypeConfiguration<OrganizationManagerRequest>
{
    public void Configure(EntityTypeBuilder<OrganizationManagerRequest> builder)
    {
        builder.ToTable("OrganizationManagerRequests");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Message)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(r => r.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(r => r.ReviewNotes)
            .HasMaxLength(500);

        builder.HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Organization)
            .WithMany()
            .HasForeignKey(r => r.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Reviewer)
            .WithMany()
            .HasForeignKey(r => r.ReviewedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(r => new { r.UserId, r.OrganizationId, r.CreatedAt });
        builder.HasIndex(r => r.Status);
    }
}

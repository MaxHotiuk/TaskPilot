using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class OrganizationInvitationConfiguration : IEntityTypeConfiguration<OrganizationInvitation>
{
    public void Configure(EntityTypeBuilder<OrganizationInvitation> builder)
    {
        builder.HasKey(oi => oi.Id);

        builder.Property(oi => oi.Role)
            .IsRequired();

        builder.Property(oi => oi.Status)
            .IsRequired();

        builder.HasOne(oi => oi.Organization)
            .WithMany()
            .HasForeignKey(oi => oi.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(oi => oi.User)
            .WithMany()
            .HasForeignKey(oi => oi.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(oi => oi.Inviter)
            .WithMany()
            .HasForeignKey(oi => oi.InvitedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(oi => new { oi.OrganizationId, oi.UserId, oi.Status });
    }
}

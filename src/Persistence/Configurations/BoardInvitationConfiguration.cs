using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class BoardInvitationConfiguration : IEntityTypeConfiguration<BoardInvitation>
{
    public void Configure(EntityTypeBuilder<BoardInvitation> builder)
    {
        builder.HasKey(bi => bi.Id);

        builder.Property(bi => bi.Role)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(bi => bi.Status)
            .IsRequired();

        builder.HasOne(bi => bi.Board)
            .WithMany()
            .HasForeignKey(bi => bi.BoardId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(bi => bi.User)
            .WithMany()
            .HasForeignKey(bi => bi.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(bi => bi.Inviter)
            .WithMany()
            .HasForeignKey(bi => bi.InvitedBy)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(bi => new { bi.BoardId, bi.UserId, bi.Status });
    }
}

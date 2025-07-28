using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class MeetingMemberConfiguration : IEntityTypeConfiguration<MeetingMember>
{
    public void Configure(EntityTypeBuilder<MeetingMember> builder)
    {
        builder.ToTable("MeetingMembers");

        builder.HasKey(mm => new { mm.MeetingId, mm.UserId });

        builder.Property(mm => mm.Status)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Invited");

        builder.Property(mm => mm.CreatedAt)
            .IsRequired();
        builder.Property(mm => mm.UpdatedAt)
            .IsRequired();

        builder.HasOne(mm => mm.Meeting)
            .WithMany(m => m.Members)
            .HasForeignKey(mm => mm.MeetingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(mm => mm.User)
            .WithMany()
            .HasForeignKey(mm => mm.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(mm => mm.UserId);
        builder.HasIndex(mm => mm.Status);
    }
}

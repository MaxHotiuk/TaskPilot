using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.Configurations;

public class BacklogConfiguration : IEntityTypeConfiguration<Backlog>
{
    public void Configure(EntityTypeBuilder<Backlog> builder)
    {
        builder.ToTable("Backlog");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Description)
            .HasColumnType("NVARCHAR(500)");

        builder.Property(b => b.BoardId)
            .IsRequired();
        
        builder.Property(b => b.CreatedAt)
            .HasColumnType("DATETIME2")
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(b => b.Board)
            .WithMany(bd => bd.Backlog)
            .HasForeignKey(b => b.BoardId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

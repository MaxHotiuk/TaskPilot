using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

public class ChatConfiguration : IEntityTypeConfiguration<Chat>
{
    public void Configure(EntityTypeBuilder<Chat> builder)
    {
        builder.ToTable("Chats");
        builder.HasKey(chat => chat.Id);

        builder.Property(chat => chat.Name)
            .HasMaxLength(200);

        builder.Property(chat => chat.Type)
            .IsRequired()
            .HasConversion<int>()
            .HasColumnType("int");

        builder.Property(chat => chat.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(chat => chat.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(chat => chat.Organization)
            .WithMany(org => org.Chats)
            .HasForeignKey(chat => chat.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(chat => chat.CreatedBy)
            .WithMany(user => user.CreatedChats)
            .HasForeignKey(chat => chat.CreatedById)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(chat => chat.Members)
            .WithOne(member => member.Chat)
            .HasForeignKey(member => member.ChatId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(chat => chat.Messages)
            .WithOne(message => message.Chat)
            .HasForeignKey(message => message.ChatId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

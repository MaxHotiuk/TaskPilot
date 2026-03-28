using Domain.Dtos.Chats;
using Domain.Entities;

namespace Application.Common.Mappings;

public static class ChatMappingExtensions
{
    public static ChatDto ToDto(this Chat chat, ChatMessage? lastMessage = null)
    {
        return new ChatDto
        {
            Id = chat.Id,
            OrganizationId = chat.OrganizationId,
            BoardId = chat.BoardId,
            Name = chat.Name,
            Type = chat.Type,
            CreatedById = chat.CreatedById,
            CreatedAt = chat.CreatedAt,
            UpdatedAt = chat.UpdatedAt,
            LastMessage = lastMessage?.ToPreviewDto(),
            Members = chat.Members.Select(member => member.ToDto())
        };
    }

    public static ChatMemberDto ToDto(this ChatMember member)
    {
        return new ChatMemberDto
        {
            UserId = member.UserId,
            Username = member.User?.Username ?? string.Empty,
            Email = member.User?.Email ?? string.Empty,
            Role = member.Role,
            LastReadAt = member.LastReadAt
        };
    }

    public static ChatMessageDto ToDto(this ChatMessage message)
    {
        return new ChatMessageDto
        {
            Id = message.Id,
            ChatId = message.ChatId,
            SenderId = message.SenderId,
            TaskId = message.TaskId,
            AssigneeId = message.AssigneeId,
            SenderName = message.Sender.Username,
            Content = message.Content,
            MessageType = message.MessageType,
            HasAttachments = message.HasAttachments,
            CreatedAt = message.CreatedAt,
            UpdatedAt = message.UpdatedAt
        };
    }

    public static ChatMessagePreviewDto ToPreviewDto(this ChatMessage message)
    {
        return new ChatMessagePreviewDto
        {
            Id = message.Id,
            SenderId = message.SenderId,
            TaskId = message.TaskId,
            AssigneeId = message.AssigneeId,
            Content = message.Content,
            MessageType = message.MessageType,
            CreatedAt = message.CreatedAt
        };
    }
}

using Application.Abstractions.Messaging;
using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using Application.Common.Mappings;
using Domain.Dtos.Chats;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Chats;

public class AddChatMembersCommandHandler : BaseCommandHandler, IRequestHandler<AddChatMembersCommand>
{
    private readonly IChatNotifier _chatNotifier;

    public AddChatMembersCommandHandler(IUnitOfWorkFactory unitOfWorkFactory, IChatNotifier chatNotifier)
        : base(unitOfWorkFactory)
    {
        _chatNotifier = chatNotifier;
    }

    public async Task Handle(AddChatMembersCommand request, CancellationToken cancellationToken)
    {
        await ExecuteInTransactionAsync(async unitOfWork =>
        {
            var chat = await unitOfWork.Chats.GetByIdAsync(request.ChatId, cancellationToken);
            if (chat is null)
            {
                throw new ValidationException($"Chat with ID {request.ChatId} does not exist");
            }

            if (chat.Type != ChatType.Group)
            {
                throw new ValidationException("Members can only be added to group chats.");
            }

            var owner = await unitOfWork.ChatMembers.GetMemberAsync(request.ChatId, request.UserId, cancellationToken);
            if (owner is null || owner.Role != ChatMemberRole.Owner)
            {
                throw new ValidationException("Only chat owners can add members.");
            }

            var newMemberIds = request.MemberIds.Distinct().Where(id => id != Guid.Empty).ToList();
            if (!newMemberIds.Any())
            {
                throw new ValidationException("Member IDs are required.");
            }

            if (!await unitOfWork.OrganizationMembers.AreMembersOfOrganizationAsync(chat.OrganizationId, newMemberIds, cancellationToken))
            {
                throw new ValidationException("All members must belong to the same organization.");
            }

            var existingMemberIds = await unitOfWork.ChatMembers.GetMemberIdsAsync(request.ChatId, cancellationToken);
            var membersToAdd = newMemberIds.Except(existingMemberIds).ToList();
            if (!membersToAdd.Any())
            {
                return;
            }

            var users = await unitOfWork.Users.FindAsync(user => membersToAdd.Contains(user.Id), cancellationToken);
            if (users.Count() != membersToAdd.Count)
            {
                throw new ValidationException("One or more members do not exist.");
            }

            foreach (var memberId in membersToAdd)
            {
                var chatMember = new ChatMember
                {
                    ChatId = chat.Id,
                    UserId = memberId,
                    Role = ChatMemberRole.Member,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await unitOfWork.ChatMembers.AddAsync(chatMember, cancellationToken);
            }

            chat.UpdatedAt = DateTime.UtcNow;
            unitOfWork.Chats.Update(chat);

            var sender = await unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
            ChatMessage? updateMessage = null;
            if (sender is not null)
            {
                var addedUsers = await unitOfWork.Users.FindAsync(user => membersToAdd.Contains(user.Id), cancellationToken);
                var addedNames = addedUsers.Select(user => user.Username ?? user.Id.ToString()).ToList();
                var content = addedNames.Any()
                    ? $"Added members: {string.Join(", ", addedNames)}."
                    : "Added members to the chat.";

                var message = new ChatMessage
                {
                    Id = Guid.NewGuid(),
                    ChatId = chat.Id,
                    SenderId = sender.Id,
                    Sender = sender,
                    Content = content,
                    MessageType = "Update",
                    HasAttachments = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                updateMessage = message;
                await unitOfWork.ChatMessages.AddAsync(message, cancellationToken);
            }

            var chatMembers = await unitOfWork.ChatMembers.GetMembersAsync(request.ChatId, cancellationToken);
            var chatDto = new ChatDto
            {
                Id = chat.Id,
                OrganizationId = chat.OrganizationId,
                BoardId = chat.BoardId,
                Name = chat.Name,
                Type = chat.Type,
                CreatedById = chat.CreatedById,
                CreatedAt = chat.CreatedAt,
                UpdatedAt = chat.UpdatedAt,
                LastMessage = updateMessage?.ToPreviewDto(),
                Members = chatMembers.Select(chatMember => chatMember.ToDto()).ToList()
            };

            if (updateMessage is not null)
            {
                await _chatNotifier.NotifyChatMessageAsync(chat.Id, updateMessage.ToDto());
            }
            await _chatNotifier.NotifyChatUpdatedAsync(chatMembers.Select(chatMember => chatMember.UserId), chatDto);
            await _chatNotifier.NotifyChatCreatedAsync(membersToAdd, chatDto);
        }, cancellationToken);
    }
}

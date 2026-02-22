using Application.Abstractions.Messaging;
using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using Application.Common.Mappings;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Chats;

public class CreateChatCommandHandler : BaseCommandHandler, IRequestHandler<CreateChatCommand, Guid>
{
    private readonly IChatNotifier _chatNotifier;

    public CreateChatCommandHandler(IUnitOfWorkFactory unitOfWorkFactory, IChatNotifier chatNotifier)
        : base(unitOfWorkFactory)
    {
        _chatNotifier = chatNotifier;
    }

    public async Task<Guid> Handle(CreateChatCommand request, CancellationToken cancellationToken)
    {
        return await ExecuteInTransactionAsync(async unitOfWork =>
        {
            var organization = await unitOfWork.Organizations.GetByIdAsync(request.OrganizationId, cancellationToken);
            if (organization is null)
            {
                throw new ValidationException($"Organization with ID {request.OrganizationId} does not exist");
            }

            var memberIds = request.MemberIds.Distinct().ToList();
            if (!memberIds.Any())
            {
                throw new ValidationException("Chat members are required.");
            }

            if (!memberIds.Contains(request.CreatedById))
            {
                memberIds.Add(request.CreatedById);
            }

            if (!await unitOfWork.OrganizationMembers.AreMembersOfOrganizationAsync(request.OrganizationId, memberIds, cancellationToken))
            {
                throw new ValidationException("All chat members must belong to the same organization.");
            }

            if (request.Type == ChatType.Private)
            {
                if (memberIds.Count != 2)
                {
                    throw new ValidationException("Private chats must include exactly two members.");
                }

                var existingChat = await unitOfWork.Chats.GetPrivateChatAsync(request.OrganizationId, memberIds[0], memberIds[1], cancellationToken);
                if (existingChat is not null)
                {
                    return existingChat.Id;
                }
            }
            else if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new ValidationException("Group chats require a name.");
            }

            var chat = new Chat
            {
                Id = Guid.NewGuid(),
                OrganizationId = request.OrganizationId,
                CreatedById = request.CreatedById,
                Name = request.Type == ChatType.Group ? request.Name?.Trim() : null,
                Type = request.Type,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await unitOfWork.Chats.AddAsync(chat, cancellationToken);

            foreach (var memberId in memberIds)
            {
                var member = new ChatMember
                {
                    ChatId = chat.Id,
                    UserId = memberId,
                    Role = memberId == request.CreatedById ? ChatMemberRole.Owner : ChatMemberRole.Member,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await unitOfWork.ChatMembers.AddAsync(member, cancellationToken);
            }

            var users = await unitOfWork.Users.FindAsync(user => memberIds.Contains(user.Id), cancellationToken);
            if (users.Count() != memberIds.Count)
            {
                throw new ValidationException("One or more chat members do not exist.");
            }
            var userLookup = users.ToDictionary(user => user.Id, user => user);
            var memberDtos = memberIds.Select(memberId =>
            {
                var user = userLookup[memberId];
                return new Domain.Dtos.Chats.ChatMemberDto
                {
                    UserId = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Role = memberId == request.CreatedById ? ChatMemberRole.Owner : ChatMemberRole.Member,
                    LastReadAt = null
                };
            });

            var chatDto = new Domain.Dtos.Chats.ChatDto
            {
                Id = chat.Id,
                OrganizationId = chat.OrganizationId,
                BoardId = chat.BoardId,
                Name = chat.Name,
                Type = chat.Type,
                CreatedById = chat.CreatedById,
                CreatedAt = chat.CreatedAt,
                UpdatedAt = chat.UpdatedAt,
                LastMessage = null,
                Members = memberDtos.ToList()
            };

            await _chatNotifier.NotifyChatCreatedAsync(memberIds, chatDto);

            return chat.Id;
        }, cancellationToken);
    }
}

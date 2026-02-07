using Application.Abstractions.Messaging;
using Application.Abstractions.Persistence;
using Application.Abstractions.Storage;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using Application.Common.Mappings;
using Domain.Dtos.Chats;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Chats;

public class ClearChatHistoryCommandHandler : BaseCommandHandler, IRequestHandler<ClearChatHistoryCommand>
{
    private readonly IAttachmentService _attachmentService;
    private readonly IChatNotifier _chatNotifier;

    public ClearChatHistoryCommandHandler(IUnitOfWorkFactory unitOfWorkFactory, IAttachmentService attachmentService, IChatNotifier chatNotifier)
        : base(unitOfWorkFactory)
    {
        _attachmentService = attachmentService;
        _chatNotifier = chatNotifier;
    }

    public async Task Handle(ClearChatHistoryCommand request, CancellationToken cancellationToken)
    {
        await ExecuteInTransactionAsync(async unitOfWork =>
        {
            var chat = await unitOfWork.Chats.GetByIdAsync(request.ChatId, cancellationToken);
            if (chat is null)
            {
                throw new ValidationException($"Chat with ID {request.ChatId} does not exist");
            }

            var member = await unitOfWork.ChatMembers.GetMemberAsync(request.ChatId, request.UserId, cancellationToken);
            if (member is null)
            {
                throw new ValidationException("User is not a member of this chat.");
            }

            if (chat.Type == ChatType.Group && member.Role != ChatMemberRole.Owner)
            {
                throw new ValidationException("Only chat owners can clear group chat history.");
            }

            var messages = await unitOfWork.ChatMessages.FindAsync(message => message.ChatId == chat.Id, cancellationToken);
            foreach (var message in messages)
            {
                var attachments = await _attachmentService.GetForEntityAsync(message.Id, cancellationToken);
                foreach (var attachment in attachments)
                {
                    await _attachmentService.DeleteAsync(attachment.FileName, cancellationToken);
                }

                unitOfWork.ChatMessages.Remove(message);
            }

            chat.UpdatedAt = DateTime.UtcNow;
            unitOfWork.Chats.Update(chat);

            var chatMembers = await unitOfWork.ChatMembers.GetMembersAsync(request.ChatId, cancellationToken);
            var chatDto = new ChatDto
            {
                Id = chat.Id,
                OrganizationId = chat.OrganizationId,
                Name = chat.Name,
                Type = chat.Type,
                CreatedById = chat.CreatedById,
                CreatedAt = chat.CreatedAt,
                UpdatedAt = chat.UpdatedAt,
                LastMessage = null,
                Members = chatMembers.Select(chatMember => chatMember.ToDto()).ToList()
            };

            await _chatNotifier.NotifyChatUpdatedAsync(chatMembers.Select(chatMember => chatMember.UserId), chatDto);
        }, cancellationToken);
    }
}

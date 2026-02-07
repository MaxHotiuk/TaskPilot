using Application.Abstractions.Messaging;
using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using Application.Common.Mappings;
using Domain.Dtos.Chats;
using MediatR;

namespace Application.Commands.Chats;

public class UpdateChatReadStatusCommandHandler : BaseCommandHandler, IRequestHandler<UpdateChatReadStatusCommand>
{
    private readonly IChatNotifier _chatNotifier;

    public UpdateChatReadStatusCommandHandler(IUnitOfWorkFactory unitOfWorkFactory, IChatNotifier chatNotifier)
        : base(unitOfWorkFactory)
    {
        _chatNotifier = chatNotifier;
    }

    public async Task Handle(UpdateChatReadStatusCommand request, CancellationToken cancellationToken)
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

            member.LastReadAt = request.ReadAt;
            member.UpdatedAt = DateTime.UtcNow;
            unitOfWork.ChatMembers.Update(member);

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

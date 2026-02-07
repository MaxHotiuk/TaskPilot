using Application.Abstractions.Persistence;
using Application.Abstractions.Storage;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Chats;

public class DeleteChatCommandHandler : BaseCommandHandler, IRequestHandler<DeleteChatCommand>
{
    private readonly IAttachmentService _attachmentService;

    public DeleteChatCommandHandler(IUnitOfWorkFactory unitOfWorkFactory, IAttachmentService attachmentService)
        : base(unitOfWorkFactory)
    {
        _attachmentService = attachmentService;
    }

    public async Task Handle(DeleteChatCommand request, CancellationToken cancellationToken)
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
                throw new ValidationException("Only chat owners can delete group chats.");
            }

            var messages = await unitOfWork.ChatMessages.FindAsync(message => message.ChatId == chat.Id, cancellationToken);
            foreach (var message in messages)
            {
                var attachments = await _attachmentService.GetForEntityAsync(message.Id, cancellationToken);
                foreach (var attachment in attachments)
                {
                    await _attachmentService.DeleteAsync(attachment.FileName, cancellationToken);
                }
            }

            unitOfWork.Chats.Remove(chat);
        }, cancellationToken);
    }
}

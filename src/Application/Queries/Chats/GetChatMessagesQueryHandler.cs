using Application.Abstractions.Persistence;
using Application.Common.Handlers;
using Application.Common.Mappings;
using Domain.Dtos.Chats;
using MediatR;

namespace Application.Queries.Chats;

public class GetChatMessagesQueryHandler : BaseQueryHandler, IRequestHandler<GetChatMessagesQuery, IEnumerable<ChatMessageDto>>
{
    public GetChatMessagesQueryHandler(IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory)
    {
    }

    public async Task<IEnumerable<ChatMessageDto>> Handle(GetChatMessagesQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            var isMember = await unitOfWork.ChatMembers.IsMemberAsync(request.ChatId, request.UserId, cancellationToken);
            if (!isMember)
            {
                return Enumerable.Empty<ChatMessageDto>();
            }

            var messages = await unitOfWork.ChatMessages.GetMessagesByChatIdAsync(request.ChatId, request.Page, request.PageSize, cancellationToken);
            return messages.Select(message => message.ToDto());
        }, cancellationToken);
    }
}

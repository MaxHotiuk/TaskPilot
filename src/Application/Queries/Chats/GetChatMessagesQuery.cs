using Domain.Dtos.Chats;
using MediatR;

namespace Application.Queries.Chats;

public record GetChatMessagesQuery(Guid ChatId, Guid UserId, int Page, int PageSize) : IRequest<IEnumerable<ChatMessageDto>>;

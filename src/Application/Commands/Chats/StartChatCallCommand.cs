using MediatR;
using Domain.Dtos.Chats;

namespace Application.Commands.Chats;

public record StartChatCallCommand(Guid ChatId, Guid SenderId) : IRequest<StartChatCallResponseDto>;

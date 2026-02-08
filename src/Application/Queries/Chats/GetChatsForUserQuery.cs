using Domain.Dtos.Chats;
using MediatR;

namespace Application.Queries.Chats;

public record GetChatsForUserQuery(Guid UserId, Guid? OrganizationId) : IRequest<IEnumerable<ChatDto>>;

using Application.Abstractions.Persistence;
using Application.Common.Handlers;
using Domain.Dtos.Chats;
using MediatR;

namespace Application.Queries.Chats;

public class GetChatsForUserQueryHandler : BaseQueryHandler, IRequestHandler<GetChatsForUserQuery, IEnumerable<ChatDto>>
{
    public GetChatsForUserQueryHandler(IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory)
    {
    }

    public async Task<IEnumerable<ChatDto>> Handle(GetChatsForUserQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            if (request.OrganizationId.HasValue)
            {
                var isMember = await unitOfWork.OrganizationMembers.IsMemberOfOrganizationAsync(request.OrganizationId.Value, request.UserId, cancellationToken);
                if (!isMember)
                {
                    return Enumerable.Empty<ChatDto>();
                }
            }

            return await unitOfWork.Chats.GetChatsForUserAsync(request.UserId, request.OrganizationId, cancellationToken);
        }, cancellationToken);
    }
}

using Application.Abstractions.Persistence;
using Application.Common.Handlers;
using Domain.Dtos.Invitations;
using MediatR;

namespace Application.Queries.Invitations;

public class GetPendingInvitationsQueryHandler : BaseCommandHandler, IRequestHandler<GetPendingInvitationsQuery, PendingInvitationsDto>
{
    public GetPendingInvitationsQueryHandler(IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory)
    {
    }

    public async Task<PendingInvitationsDto> Handle(GetPendingInvitationsQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteInTransactionAsync(async unitOfWork =>
        {
            var boardInvitations = await unitOfWork.BoardInvitations.GetPendingInvitationsByUserIdAsync(request.UserId, cancellationToken);
            var organizationInvitations = await unitOfWork.OrganizationInvitations.GetPendingInvitationsByUserIdAsync(request.UserId, cancellationToken);

            var result = new PendingInvitationsDto
            {
                BoardInvitations = boardInvitations.Select(bi => new BoardInvitationDto
                {
                    Id = bi.Id,
                    BoardId = bi.BoardId,
                    BoardName = bi.Board?.Name ?? string.Empty,
                    InvitedBy = bi.InvitedBy,
                    InviterName = bi.Inviter?.Username ?? bi.Inviter?.Email ?? string.Empty,
                    Role = bi.Role,
                    CreatedAt = bi.CreatedAt
                }).ToList(),

                OrganizationInvitations = organizationInvitations.Select(oi => new OrganizationInvitationDto
                {
                    Id = oi.Id,
                    OrganizationId = oi.OrganizationId,
                    OrganizationName = oi.Organization?.Name ?? string.Empty,
                    InvitedBy = oi.InvitedBy,
                    InviterName = oi.Inviter?.Username ?? oi.Inviter?.Email ?? string.Empty,
                    Role = oi.Role.ToString(),
                    CreatedAt = oi.CreatedAt
                }).ToList()
            };

            return result;
        }, cancellationToken);
    }
}

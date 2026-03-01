using Domain.Dtos.Invitations;
using MediatR;

namespace Application.Queries.Invitations;

public record GetPendingInvitationsQuery(Guid UserId) : IRequest<PendingInvitationsDto>;

public class PendingInvitationsDto
{
    public List<BoardInvitationDto> BoardInvitations { get; set; } = new();
    public List<OrganizationInvitationDto> OrganizationInvitations { get; set; } = new();
}

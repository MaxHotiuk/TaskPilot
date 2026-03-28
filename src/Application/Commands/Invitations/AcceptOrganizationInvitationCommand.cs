using MediatR;

namespace Application.Commands.Invitations;

public record AcceptOrganizationInvitationCommand(Guid InvitationId, Guid CurrentUserId) : IRequest;

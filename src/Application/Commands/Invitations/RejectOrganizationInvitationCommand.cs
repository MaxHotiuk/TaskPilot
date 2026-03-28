using MediatR;

namespace Application.Commands.Invitations;

public record RejectOrganizationInvitationCommand(Guid InvitationId, Guid CurrentUserId) : IRequest;

using MediatR;

namespace Application.Commands.Invitations;

public record RejectBoardInvitationCommand(Guid InvitationId, Guid CurrentUserId) : IRequest;

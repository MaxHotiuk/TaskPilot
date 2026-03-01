using MediatR;

namespace Application.Commands.Invitations;

public record AcceptBoardInvitationCommand(Guid InvitationId, Guid CurrentUserId) : IRequest;

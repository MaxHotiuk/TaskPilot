using MediatR;

namespace Application.Commands.UserProfile;

public record DeleteUserProfileCommand(Guid Id) : IRequest;

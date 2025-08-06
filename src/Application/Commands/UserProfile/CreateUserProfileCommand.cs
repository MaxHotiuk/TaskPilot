using MediatR;

namespace Application.Commands.UserProfile;

public record CreateUserProfileCommand(
    Guid UserId,
    string? Bio = null,
    string? JobTitle = null,
    string? Department = null,
    string? Location = null,
    string? PhoneNumber = null,
    bool AddToBoardAutomatically = false,
    bool ShowEmail = false,
    bool ShowPhoneNumber = false
) : IRequest<Guid>;

using MediatR;

namespace Application.Commands.UserProfile;

public record UpdateUserProfileCommand(
    Guid Id,
    string? Bio = null,
    string? JobTitle = null,
    string? Department = null,
    string? Location = null,
    string? PhoneNumber = null,
    bool? AddToBoardAutomatically = null,
    bool? ShowEmail = null,
    bool? ShowPhoneNumber = null
) : IRequest;

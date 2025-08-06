namespace Domain.Dtos.UserProfiles;

public record UpdateUserProfileRequestDto(
    string? Bio = null,
    string? JobTitle = null,
    string? Department = null,
    string? Location = null,
    string? PhoneNumber = null,
    bool? AddToBoardAutomatically = null,
    bool? ShowEmail = null,
    bool? ShowPhoneNumber = null
);

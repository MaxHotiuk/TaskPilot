using Domain.Dtos.UserProfiles;
using Domain.Entities;

namespace Application.Common.Mappings;

public static class UserProfileMappingExtensions
{
    public static UserProfileDto ToDto(this UserProfile userProfile)
    {
        return new UserProfileDto
        {
            Id = userProfile.Id,
            UserId = userProfile.UserId,
            Bio = userProfile.Bio,
            JobTitle = userProfile.JobTitle,
            Department = userProfile.Department,
            Location = userProfile.Location,
            PhoneNumber = userProfile.PhoneNumber,
            AddToBoardAutomatically = userProfile.AddToBoardAutomatically,
            ShowEmail = userProfile.ShowEmail,
            ShowPhoneNumber = userProfile.ShowPhoneNumber,
            CreatedAt = userProfile.CreatedAt,
            UpdatedAt = userProfile.UpdatedAt
        };
    }

    public static IEnumerable<UserProfileDto> ToDto(this IEnumerable<UserProfile> userProfiles)
    {
        return userProfiles.Select(ToDto);
    }
}

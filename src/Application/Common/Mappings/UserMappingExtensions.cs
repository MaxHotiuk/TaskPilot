using Domain.Dtos.Users;
using Domain.Entities;

namespace Application.Common.Mappings;

public static class UserMappingExtensions
{
    public static UserDto ToDto(this User user)
    {
        return new UserDto
        {
            Id = user.Id,
            EntraId = user.EntraId,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    public static IEnumerable<UserDto> ToDto(this IEnumerable<User> users)
    {
        return users.Select(ToDto);
    }
}

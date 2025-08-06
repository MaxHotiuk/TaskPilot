using Domain.Dtos.UserProfiles;
using MediatR;

namespace Application.Queries.UserProfile;

public record GetUserProfilesByDepartmentQuery(string Department) : IRequest<IEnumerable<UserProfileDto>>;

using Domain.Dtos.UserProfiles;
using MediatR;

namespace Application.Queries.UserProfile;

public record GetAllUserProfilesQuery() : IRequest<IEnumerable<UserProfileDto>>;

using Domain.Dtos.UserProfiles;
using MediatR;

namespace Application.Queries.UserProfile;

public record GetUserProfilesByLocationQuery(string Location) : IRequest<IEnumerable<UserProfileDto>>;

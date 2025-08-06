using Domain.Dtos.UserProfiles;
using MediatR;

namespace Application.Queries.UserProfile;

public record GetUserProfileByUserIdQuery(Guid UserId) : IRequest<UserProfileDto?>;

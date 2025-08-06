using Domain.Dtos.UserProfiles;
using MediatR;

namespace Application.Queries.UserProfile;

public record GetUserProfileByIdQuery(Guid Id) : IRequest<UserProfileDto?>;

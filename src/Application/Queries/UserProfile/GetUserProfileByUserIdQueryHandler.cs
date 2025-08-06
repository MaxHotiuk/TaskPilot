using Application.Abstractions.Persistence;
using Application.Common.Handlers;
using Application.Common.Mappings;
using Domain.Dtos.UserProfiles;
using MediatR;

namespace Application.Queries.UserProfile;

public class GetUserProfileByUserIdQueryHandler : BaseCommandHandler, IRequestHandler<GetUserProfileByUserIdQuery, UserProfileDto?>
{
    public GetUserProfileByUserIdQueryHandler(IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory)
    {
    }

    public async Task<UserProfileDto?> Handle(GetUserProfileByUserIdQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteInTransactionAsync(async unitOfWork =>
        {
            var userProfile = await unitOfWork.UserProfiles.GetByUserIdAsync(request.UserId, cancellationToken);
            
            if (userProfile != null)
            {
                return userProfile.ToDto();
            }

            var user = await unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                return null;
            }

            var newUserProfile = new Domain.Entities.UserProfile
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Bio = null,
                JobTitle = null,
                Department = null,
                Location = null,
                PhoneNumber = null,
                AddToBoardAutomatically = false,
                ShowEmail = false,
                ShowPhoneNumber = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await unitOfWork.UserProfiles.AddAsync(newUserProfile, cancellationToken);
            
            return newUserProfile.ToDto();
        }, cancellationToken);
    }
}

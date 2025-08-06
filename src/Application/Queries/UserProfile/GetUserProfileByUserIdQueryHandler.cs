using Application.Abstractions.Persistence;
using Application.Common.Handlers;
using Application.Common.Mappings;
using Domain.Dtos.UserProfiles;
using MediatR;

namespace Application.Queries.UserProfile;

public class GetUserProfileByUserIdQueryHandler : BaseQueryHandler, IRequestHandler<GetUserProfileByUserIdQuery, UserProfileDto?>
{
    public GetUserProfileByUserIdQueryHandler(IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory)
    {
    }

    public async Task<UserProfileDto?> Handle(GetUserProfileByUserIdQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            var userProfile = await unitOfWork.UserProfiles.GetByUserIdAsync(request.UserId, cancellationToken);
            return userProfile?.ToDto();
        }, cancellationToken);
    }
}

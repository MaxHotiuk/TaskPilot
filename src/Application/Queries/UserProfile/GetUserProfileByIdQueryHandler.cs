using Application.Abstractions.Persistence;
using Application.Common.Handlers;
using Application.Common.Mappings;
using Domain.Dtos.UserProfiles;
using MediatR;

namespace Application.Queries.UserProfile;

public class GetUserProfileByIdQueryHandler : BaseQueryHandler, IRequestHandler<GetUserProfileByIdQuery, UserProfileDto?>
{
    public GetUserProfileByIdQueryHandler(IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory)
    {
    }

    public async Task<UserProfileDto?> Handle(GetUserProfileByIdQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            var userProfile = await unitOfWork.UserProfiles.GetByIdAsync(request.Id, cancellationToken);
            return userProfile?.ToDto();
        }, cancellationToken);
    }
}

using Application.Abstractions.Persistence;
using Application.Common.Handlers;
using Application.Common.Mappings;
using Domain.Dtos.UserProfiles;
using MediatR;

namespace Application.Queries.UserProfile;

public class GetAllUserProfilesQueryHandler : BaseQueryHandler, IRequestHandler<GetAllUserProfilesQuery, IEnumerable<UserProfileDto>>
{
    public GetAllUserProfilesQueryHandler(IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory)
    {
    }

    public async Task<IEnumerable<UserProfileDto>> Handle(GetAllUserProfilesQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            var userProfiles = await unitOfWork.UserProfiles.GetAllAsync(cancellationToken);
            return userProfiles.ToDto();
        }, cancellationToken);
    }
}

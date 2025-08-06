using Application.Abstractions.Persistence;
using Application.Common.Handlers;
using Application.Common.Mappings;
using Domain.Dtos.UserProfiles;
using MediatR;

namespace Application.Queries.UserProfile;

public class GetUserProfilesByLocationQueryHandler : BaseQueryHandler, IRequestHandler<GetUserProfilesByLocationQuery, IEnumerable<UserProfileDto>>
{
    public GetUserProfilesByLocationQueryHandler(IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory)
    {
    }

    public async Task<IEnumerable<UserProfileDto>> Handle(GetUserProfilesByLocationQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            var userProfiles = await unitOfWork.UserProfiles.GetByLocationAsync(request.Location, cancellationToken);
            return userProfiles.ToDto();
        }, cancellationToken);
    }
}

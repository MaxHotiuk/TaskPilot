using Application.Abstractions.Persistence;
using Application.Common.Handlers;
using Application.Common.Mappings;
using Domain.Dtos.UserProfiles;
using MediatR;

namespace Application.Queries.UserProfile;

public class GetUserProfilesByDepartmentQueryHandler : BaseQueryHandler, IRequestHandler<GetUserProfilesByDepartmentQuery, IEnumerable<UserProfileDto>>
{
    public GetUserProfilesByDepartmentQueryHandler(IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory)
    {
    }

    public async Task<IEnumerable<UserProfileDto>> Handle(GetUserProfilesByDepartmentQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            var userProfiles = await unitOfWork.UserProfiles.GetByDepartmentAsync(request.Department, cancellationToken);
            return userProfiles.ToDto();
        }, cancellationToken);
    }
}

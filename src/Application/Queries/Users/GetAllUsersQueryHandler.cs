using Application.Abstractions.Persistence;
using Domain.Dtos.Users;
using Application.Common.Handlers;
using Application.Common.Mappings;
using MediatR;

namespace Application.Queries.Users;

public class GetAllUsersQueryHandler : BaseQueryHandler, IRequestHandler<GetAllUsersQuery, IEnumerable<UserDto>>
{
    public GetAllUsersQueryHandler(IUnitOfWorkFactory unitOfWorkFactory) 
        : base(unitOfWorkFactory)
    {
    }

    public async Task<IEnumerable<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            var users = await unitOfWork.Users.GetAllAsync(cancellationToken);
            return users.ToDto();
        }, cancellationToken);
    }
}

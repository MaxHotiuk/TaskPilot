using Application.Abstractions.Persistence;
using Application.Common.Dtos.Users;
using Application.Common.Handlers;
using Application.Common.Mappings;
using MediatR;

namespace Application.Queries.Users;

public class GetUserByEntraIdQueryHandler : BaseQueryHandler, IRequestHandler<GetUserByEntraIdQuery, UserDto?>
{
    public GetUserByEntraIdQueryHandler(IUnitOfWorkFactory unitOfWorkFactory) 
        : base(unitOfWorkFactory)
    {
    }

    public async Task<UserDto?> Handle(GetUserByEntraIdQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            var user = await unitOfWork.Users.GetByEntraIdAsync(request.EntraId, cancellationToken);
            return user?.ToDto();
        }, cancellationToken);
    }
}

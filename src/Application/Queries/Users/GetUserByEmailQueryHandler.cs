using Application.Abstractions.Persistence;
using Application.Common.Dtos.Users;
using Application.Common.Handlers;
using Application.Common.Mappings;
using MediatR;

namespace Application.Queries.Users;

public class GetUserByEmailQueryHandler : BaseQueryHandler, IRequestHandler<GetUserByEmailQuery, UserDto?>
{
    public GetUserByEmailQueryHandler(IUnitOfWorkFactory unitOfWorkFactory) 
        : base(unitOfWorkFactory)
    {
    }

    public async Task<UserDto?> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            var user = await unitOfWork.Users.GetByEmailAsync(request.Email, cancellationToken);
            return user?.ToDto();
        }, cancellationToken);
    }
}

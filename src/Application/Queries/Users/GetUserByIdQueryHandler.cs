using Application.Abstractions.Persistence;
using Domain.Dtos.Users;
using Application.Common.Handlers;
using Application.Common.Mappings;
using MediatR;

namespace Application.Queries.Users;

public class GetUserByIdQueryHandler : BaseQueryHandler, IRequestHandler<GetUserByIdQuery, UserDto?>
{
    public GetUserByIdQueryHandler(IUnitOfWorkFactory unitOfWorkFactory) 
        : base(unitOfWorkFactory)
    {
    }

    public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            var user = await unitOfWork.Users.GetByIdAsync(request.Id, cancellationToken);
            return user?.ToDto();
        }, cancellationToken);
    }
}

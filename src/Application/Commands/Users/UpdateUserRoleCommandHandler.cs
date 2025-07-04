using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using Domain.Common.Authorization;
using Domain.Entities;
using MediatR;

namespace Application.Commands.Users;

public class UpdateUserRoleCommandHandler : BaseCommandHandler, IRequestHandler<UpdateUserRoleCommand>
{
    public UpdateUserRoleCommandHandler(IUnitOfWorkFactory unitOfWorkFactory) 
        : base(unitOfWorkFactory)
    {
    }

    public async Task Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
    {
        await ExecuteInTransactionAsync(async unitOfWork =>
        {
            var user = await unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
            
            if (user == null)
            {
                throw new NotFoundException(nameof(User), request.UserId);
            }

            if (!Roles.All.Contains(request.Role))
            {
                throw new ValidationException($"Invalid role: {request.Role}. Valid roles are: {string.Join(", ", Roles.All)}");
            }

            user.Role = request.Role;
            user.UpdatedAt = DateTime.UtcNow;

            unitOfWork.Users.Update(user);
        }, cancellationToken);
    }
}

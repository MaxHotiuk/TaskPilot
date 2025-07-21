using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using MediatR;

namespace Application.Commands.Users;

public class UpdateUserCommandHandler : BaseCommandHandler, IRequestHandler<UpdateUserCommand>
{
    public UpdateUserCommandHandler(IUnitOfWorkFactory unitOfWorkFactory) 
        : base(unitOfWorkFactory)
    {
    }

    public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        await ExecuteInTransactionAsync(async unitOfWork =>
        {
            var user = await unitOfWork.Users.GetByIdAsync(request.Id, cancellationToken);
            
            if (user == null)
            {
                throw new NotFoundException(nameof(Domain.Entities.User), request.Id);
            }

            var existingUser = await unitOfWork.Users.GetByEmailAsync(request.Email, cancellationToken);
            if (existingUser != null && existingUser.Id != request.Id)
            {
                throw new ValidationException("Email is already taken by another user");
            }

            user.Email = request.Email;
            user.Username = request.Username;
            user.Role = request.Role;
            user.UpdatedAt = DateTime.UtcNow;

            unitOfWork.Users.Update(user);
        }, cancellationToken);
    }
}

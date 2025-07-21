using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using MediatR;

namespace Application.Commands.Users;

public class DeleteUserCommandHandler : BaseCommandHandler, IRequestHandler<DeleteUserCommand>
{
    public DeleteUserCommandHandler(IUnitOfWorkFactory unitOfWorkFactory) 
        : base(unitOfWorkFactory)
    {
    }

    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        await ExecuteInTransactionAsync(async unitOfWork =>
        {
            var user = await unitOfWork.Users.GetByIdAsync(request.Id, cancellationToken);
            
            if (user == null)
            {
                throw new NotFoundException(nameof(Domain.Entities.User), request.Id);
            }

            unitOfWork.Users.Remove(user);
        }, cancellationToken);
    }
}

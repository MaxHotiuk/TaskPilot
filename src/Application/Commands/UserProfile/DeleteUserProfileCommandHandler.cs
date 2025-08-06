using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using MediatR;

namespace Application.Commands.UserProfile;

public class DeleteUserProfileCommandHandler : BaseCommandHandler, IRequestHandler<DeleteUserProfileCommand>
{
    public DeleteUserProfileCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory)
    {
    }

    public async Task Handle(DeleteUserProfileCommand request, CancellationToken cancellationToken)
    {
        await ExecuteInTransactionAsync(async unitOfWork =>
        {
            var userProfile = await unitOfWork.UserProfiles.GetByIdAsync(request.Id, cancellationToken);
            
            if (userProfile == null)
            {
                throw new NotFoundException(nameof(Domain.Entities.UserProfile), request.Id);
            }

            unitOfWork.UserProfiles.Remove(userProfile);
        }, cancellationToken);
    }
}

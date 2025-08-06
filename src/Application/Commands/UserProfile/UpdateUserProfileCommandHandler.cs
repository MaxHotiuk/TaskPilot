using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using MediatR;

namespace Application.Commands.UserProfile;

public class UpdateUserProfileCommandHandler : BaseCommandHandler, IRequestHandler<UpdateUserProfileCommand>
{
    public UpdateUserProfileCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory)
    {
    }

    public async Task Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        await ExecuteInTransactionAsync(async unitOfWork =>
        {
            var userProfile = await unitOfWork.UserProfiles.GetByIdAsync(request.Id, cancellationToken);
            
            if (userProfile == null)
            {
                throw new NotFoundException(nameof(Domain.Entities.UserProfile), request.Id);
            }

            if (request.Bio != null)
                userProfile.Bio = request.Bio;
            
            if (request.JobTitle != null)
                userProfile.JobTitle = request.JobTitle;
            
            if (request.Department != null)
                userProfile.Department = request.Department;
            
            if (request.Location != null)
                userProfile.Location = request.Location;
            
            if (request.PhoneNumber != null)
                userProfile.PhoneNumber = request.PhoneNumber;
            
            if (request.AddToBoardAutomatically.HasValue)
                userProfile.AddToBoardAutomatically = request.AddToBoardAutomatically.Value;
            
            if (request.ShowEmail.HasValue)
                userProfile.ShowEmail = request.ShowEmail.Value;
            
            if (request.ShowPhoneNumber.HasValue)
                userProfile.ShowPhoneNumber = request.ShowPhoneNumber.Value;

            userProfile.UpdatedAt = DateTime.UtcNow;

            unitOfWork.UserProfiles.Update(userProfile);
        }, cancellationToken);
    }
}

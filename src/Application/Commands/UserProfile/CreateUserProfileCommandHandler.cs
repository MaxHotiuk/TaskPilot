using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using Domain.Entities;
using MediatR;

namespace Application.Commands.UserProfile;

public class CreateUserProfileCommandHandler : BaseCommandHandler, IRequestHandler<CreateUserProfileCommand, Guid>
{
    public CreateUserProfileCommandHandler(IUnitOfWorkFactory unitOfWorkFactory)
        : base(unitOfWorkFactory)
    {
    }

    public async Task<Guid> Handle(CreateUserProfileCommand request, CancellationToken cancellationToken)
    {
        return await ExecuteInTransactionAsync(async unitOfWork =>
        {
            var user = await unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken);
            if (user == null)
            {
                throw new NotFoundException(nameof(Domain.Entities.User), request.UserId);
            }

            var existingProfile = await unitOfWork.UserProfiles.GetByUserIdAsync(request.UserId, cancellationToken);
            if (existingProfile != null)
            {
                throw new ValidationException("User profile already exists for this user");
            }

            var userProfile = new Domain.Entities.UserProfile
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Bio = request.Bio,
                JobTitle = request.JobTitle,
                Department = request.Department,
                Location = request.Location,
                PhoneNumber = request.PhoneNumber,
                AddToBoardAutomatically = request.AddToBoardAutomatically,
                ShowEmail = request.ShowEmail,
                ShowPhoneNumber = request.ShowPhoneNumber,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await unitOfWork.UserProfiles.AddAsync(userProfile, cancellationToken);

            return userProfile.Id;
        }, cancellationToken);
    }
}

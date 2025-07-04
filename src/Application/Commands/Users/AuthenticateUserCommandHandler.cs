using Application.Abstractions.Persistence;
using Application.Common.Dtos.Users;
using Application.Common.Handlers;
using Application.Common.Mappings;
using Domain.Entities;
using MediatR;

namespace Application.Commands.Users;

public class AuthenticateUserCommandHandler : BaseCommandHandler, IRequestHandler<AuthenticateUserCommand, UserDto>
{
    public AuthenticateUserCommandHandler(IUnitOfWorkFactory unitOfWorkFactory) 
        : base(unitOfWorkFactory)
    {
    }

    public async Task<UserDto> Handle(AuthenticateUserCommand request, CancellationToken cancellationToken)
    {
        return await ExecuteInTransactionAsync(async unitOfWork =>
        {
            var existingUser = await unitOfWork.Users.GetByEntraIdAsync(request.EntraId, cancellationToken);
            
            if (existingUser != null)
            {
                if (existingUser.Email != request.Email || existingUser.Username != request.Username)
                {
                    existingUser.Email = request.Email;
                    existingUser.Username = request.Username;
                    existingUser.UpdatedAt = DateTime.UtcNow;
                    
                    unitOfWork.Users.Update(existingUser);
                }
                
                return existingUser.ToDto();
            }

            var newUser = new User
            {
                Id = Guid.NewGuid(),
                EntraId = request.EntraId,
                Email = request.Email,
                Username = request.Username,
                Role = "User",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await unitOfWork.Users.AddAsync(newUser, cancellationToken);

            return newUser.ToDto();
        }, cancellationToken);
    }
}

using Application.Abstractions.Persistence;
using Application.Common.Dtos.Users;
using Application.Common.Mappings;
using Domain.Entities;
using MediatR;

namespace Application.Commands.Users;

public class AuthenticateUserCommandHandler : IRequestHandler<AuthenticateUserCommand, UserDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AuthenticateUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UserDto> Handle(AuthenticateUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetByEntraIdAsync(request.EntraId, cancellationToken);
        
        if (existingUser != null)
        {
            if (existingUser.Email != request.Email || existingUser.Username != request.Username)
            {
                existingUser.Email = request.Email;
                existingUser.Username = request.Username;
                existingUser.UpdatedAt = DateTime.UtcNow;
                
                _userRepository.Update(existingUser);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
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

        await _userRepository.AddAsync(newUser, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return newUser.ToDto();
    }
}

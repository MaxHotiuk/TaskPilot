using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Domain.Entities;
using MediatR;

namespace Application.Commands.Users;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Check if user already exists by email
        if (await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken))
        {
            throw new ValidationException("User with this email already exists");
        }

        // Check if user already exists by EntraId
        if (await _userRepository.ExistsByEntraIdAsync(request.EntraId, cancellationToken))
        {
            throw new ValidationException("User with this EntraId already exists");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Username = request.Username,
            EntraId = request.EntraId,
            Role = request.Role,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}

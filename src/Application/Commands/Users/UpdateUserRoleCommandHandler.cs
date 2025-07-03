using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Domain.Common.Authorization;
using Domain.Entities;
using MediatR;

namespace Application.Commands.Users;

public class UpdateUserRoleCommandHandler : IRequestHandler<UpdateUserRoleCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserRoleCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        
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

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

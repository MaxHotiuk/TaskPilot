using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Domain.Entities;
using MediatR;

namespace Application.Commands.BoardMembers;

public class AddBoardMemberCommandHandler : IRequestHandler<AddBoardMemberCommand>
{
    private readonly IBoardMemberRepository _boardMemberRepository;
    private readonly IBoardRepository _boardRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddBoardMemberCommandHandler(
        IBoardMemberRepository boardMemberRepository,
        IBoardRepository boardRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _boardMemberRepository = boardMemberRepository;
        _boardRepository = boardRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(AddBoardMemberCommand request, CancellationToken cancellationToken)
    {
        // Validate board exists
        var board = await _boardRepository.GetByIdAsync(request.BoardId, cancellationToken);
        if (board is null)
        {
            throw new ValidationException($"Board with ID {request.BoardId} does not exist");
        }

        // Validate user exists
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            throw new ValidationException($"User with ID {request.UserId} does not exist");
        }

        // Check if user is already a member
        if (await _boardMemberRepository.IsMemberOfBoardAsync(request.BoardId, request.UserId, cancellationToken))
        {
            throw new ValidationException($"User {request.UserId} is already a member of board {request.BoardId}");
        }

        var boardMember = new BoardMember
        {
            BoardId = request.BoardId,
            UserId = request.UserId,
            Role = request.Role,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _boardMemberRepository.AddAsync(boardMember, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

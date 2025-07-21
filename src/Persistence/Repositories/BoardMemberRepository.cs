using Application.Abstractions.Persistence;
using Database;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repositories;

public class BoardMemberRepository : Repository<BoardMember, object>, IBoardMemberRepository
{
    public BoardMemberRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<BoardMember>> GetMembersByBoardIdAsync(Guid boardId, CancellationToken cancellationToken = default)
    {
        var members = await Context.BoardMembers
            .Where(bm => bm.BoardId == boardId)
            .Include(bm => bm.User)
            .ToListAsync(cancellationToken);

        var board = await Context.Boards
            .Include(b => b.Owner)
            .FirstOrDefaultAsync(b => b.Id == boardId, cancellationToken);

        if (board != null && !members.Any(m => m.UserId == board.OwnerId))
        {
            members.Insert(0, new BoardMember
            {
                BoardId = board.Id,
                UserId = board.OwnerId,
                Role = "Owner",
                User = board.Owner,
                Board = board
            });
        }

        return members;
    }

    public async Task<IEnumerable<BoardMember>> GetBoardsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var boardsAsMember = await Context.BoardMembers
            .Where(bm => bm.UserId == userId)
            .Include(bm => bm.Board)
            .ToListAsync(cancellationToken);

        var ownedBoards = await Context.Boards
            .Include(b => b.Owner)
            .Where(b => b.OwnerId == userId)
            .ToListAsync(cancellationToken);

        foreach (var ownedBoard in ownedBoards)
        {
            if (!boardsAsMember.Any(bm => bm.BoardId == ownedBoard.Id))
            {
                boardsAsMember.Insert(0, new BoardMember
                {
                    BoardId = ownedBoard.Id,
                    UserId = userId,
                    Role = "Owner",
                    User = ownedBoard.Owner,
                    Board = ownedBoard
                });
            }
        }

        return boardsAsMember;
    }

    public async Task<BoardMember?> GetBoardMemberAsync(Guid boardId, Guid userId, CancellationToken cancellationToken = default)
    {
        var member = await Context.BoardMembers
            .FirstOrDefaultAsync(bm => bm.BoardId == boardId && bm.UserId == userId, cancellationToken);

        if (member != null)
            return member;

        var board = await Context.Boards
            .Include(b => b.Owner)
            .FirstOrDefaultAsync(b => b.Id == boardId, cancellationToken);

        if (board != null && board.OwnerId == userId)
        {
            return new BoardMember
            {
                BoardId = board.Id,
                UserId = userId,
                Role = "Owner",
                User = board.Owner,
                Board = board
            };
        }

        return null;
    }

    public async Task<bool> IsMemberOfBoardAsync(Guid boardId, Guid userId, CancellationToken cancellationToken = default)
    {
        var isMember = await Context.BoardMembers
            .AnyAsync(bm => bm.BoardId == boardId && bm.UserId == userId, cancellationToken);
        if (isMember)
            return true;

        var board = await Context.Boards
            .FirstOrDefaultAsync(b => b.Id == boardId, cancellationToken);
        return board != null && board.OwnerId == userId;
    }
}

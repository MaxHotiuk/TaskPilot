using Domain.Dtos.Boards;
using Domain.Entities;

namespace Application.Common.Mappings;

public static class BoardMappingExtensions
{
    public static BoardDto ToDto(this Board board)
    {
        return new BoardDto
        {
            Id = board.Id,
            Name = board.Name,
            Description = board.Description,
            OwnerId = board.OwnerId,
            CreatedAt = board.CreatedAt,
            UpdatedAt = board.UpdatedAt
        };
    }

    public static IEnumerable<BoardDto> ToDto(this IEnumerable<Board> boards)
    {
        return boards.Select(ToDto);
    }

    public static BoardSearchDto ToSearchDto(this Board board)
    {
        return new BoardSearchDto
        {
            Id = board.Id,
            Name = board.Name,
            Description = board.Description,
            NumberOfMembers = board.Members.Count,
            NumberOfTasks = board.Tasks.Count,
            OwnerId = board.OwnerId,
            CreatedAt = board.CreatedAt,
            UpdatedAt = board.UpdatedAt
        };
    }

    public static IEnumerable<BoardSearchDto> ToSearchDto(this IEnumerable<Board> boards)
    {
        return boards.Select(ToSearchDto);
    }
}

using Domain.Dtos.BoardMembers;
using Domain.Entities;

namespace Application.Common.Mappings;

public static class BoardMemberMappingExtensions
{
    public static BoardMemberDto ToDto(this BoardMember boardMember)
    {
        return new BoardMemberDto
        {
            BoardId = boardMember.BoardId,
            UserId = boardMember.UserId,
            Role = boardMember.Role,
            CreatedAt = boardMember.CreatedAt,
            UpdatedAt = boardMember.UpdatedAt
        };
    }

    public static IEnumerable<BoardMemberDto> ToDto(this IEnumerable<BoardMember> boardMembers)
    {
        return boardMembers.Select(ToDto);
    }
}

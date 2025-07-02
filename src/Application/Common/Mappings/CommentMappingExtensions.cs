using Application.Common.Dtos.Comments;
using Domain.Entities;

namespace Application.Common.Mappings;

public static class CommentMappingExtensions
{
    public static CommentDto ToDto(this Comment comment)
    {
        return new CommentDto
        {
            Id = comment.Id,
            TaskId = comment.TaskId,
            AuthorId = comment.AuthorId,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt
        };
    }

    public static IEnumerable<CommentDto> ToDto(this IEnumerable<Comment> comments)
    {
        return comments.Select(ToDto);
    }
}

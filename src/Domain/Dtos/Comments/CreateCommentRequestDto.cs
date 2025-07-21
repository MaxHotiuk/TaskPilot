namespace Domain.Dtos.Comments;

public class CreateCommentRequestDto
{
    public Guid TaskId { get; set; }
    public Guid AuthorId { get; set; }
    public string Content { get; set; } = string.Empty;
}

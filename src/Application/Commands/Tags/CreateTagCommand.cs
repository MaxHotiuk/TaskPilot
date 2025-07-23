using MediatR;

namespace Application.Commands.Tags;

public class CreateTagCommand : IRequest<int>
{
    public Guid BoardId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Color { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

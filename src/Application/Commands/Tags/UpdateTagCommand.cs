using MediatR;

namespace Application.Commands.Tags;

public class UpdateTagCommand : IRequest
{
    public int Id { get; set; }
    public Guid BoardId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Color { get; set; }
}

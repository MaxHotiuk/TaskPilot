using MediatR;

namespace Application.Commands.Tags;

public class DeleteTagCommand : IRequest
{
    public int Id { get; set; }
    public Guid BoardId { get; set; }
}

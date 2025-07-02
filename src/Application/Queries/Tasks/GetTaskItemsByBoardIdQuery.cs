using Application.Common.Dtos.Tasks;
using MediatR;

namespace Application.Queries.Tasks;

public record GetTaskItemsByBoardIdQuery(Guid BoardId) : IRequest<IEnumerable<TaskItemDto>>;

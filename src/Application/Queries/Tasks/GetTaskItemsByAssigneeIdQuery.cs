using Application.Common.Dtos.Tasks;
using MediatR;

namespace Application.Queries.Tasks;

public record GetTaskItemsByAssigneeIdQuery(Guid AssigneeId) : IRequest<IEnumerable<TaskItemDto>>;

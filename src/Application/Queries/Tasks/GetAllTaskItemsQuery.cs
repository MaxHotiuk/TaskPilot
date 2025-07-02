using Application.Common.Dtos.Tasks;
using MediatR;

namespace Application.Queries.Tasks;

public record GetAllTaskItemsQuery : IRequest<IEnumerable<TaskItemDto>>;

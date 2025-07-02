using Application.Common.Dtos.Tasks;
using MediatR;

namespace Application.Queries.Tasks;

public record GetOverdueTaskItemsQuery : IRequest<IEnumerable<TaskItemDto>>;

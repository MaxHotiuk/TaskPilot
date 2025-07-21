using Domain.Dtos.Tasks;
using MediatR;

namespace Application.Queries.Tasks;

public record GetOverdueTaskItemsQuery : IRequest<IEnumerable<TaskItemDto>>;

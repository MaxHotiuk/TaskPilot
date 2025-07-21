using Domain.Dtos.Tasks;
using MediatR;

namespace Application.Queries.Tasks;

public record GetTaskItemByIdQuery(Guid Id) : IRequest<TaskItemDto?>;

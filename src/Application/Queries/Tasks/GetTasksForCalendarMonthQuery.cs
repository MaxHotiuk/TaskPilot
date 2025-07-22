using Domain.Dtos.Tasks;
using MediatR;

namespace Application.Queries.Tasks;

public record GetTasksForCalendarMonthQuery(Guid UserId, DateTime DayInMonth) : IRequest<IEnumerable<TaskCalendarItemDto>>;

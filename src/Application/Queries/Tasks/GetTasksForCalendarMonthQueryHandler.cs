using Application.Abstractions.Persistence;
using Domain.Dtos.Tasks;
using Application.Common.Handlers;
using Application.Common.Mappings;
using MediatR;

namespace Application.Queries.Tasks;

public class GetTasksForCalendarMonthQueryHandler : BaseQueryHandler, IRequestHandler<GetTasksForCalendarMonthQuery, IEnumerable<TaskCalendarItemDto>>
{
    public GetTasksForCalendarMonthQueryHandler(IUnitOfWorkFactory unitOfWorkFactory) 
        : base(unitOfWorkFactory)
    {
    }

    public async Task<IEnumerable<TaskCalendarItemDto>> Handle(GetTasksForCalendarMonthQuery request, CancellationToken cancellationToken)
    {
        var startDate = new DateTime(request.DayInMonth.Year, request.DayInMonth.Month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);
        return await ExecuteQueryAsync(async unitOfWork =>
        {
            return await unitOfWork.Tasks.GetTasksForCalendarAsync(request.UserId, startDate, endDate, cancellationToken);
        }, cancellationToken);
    }
}

using Application.Abstractions.Calendar;
using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using Application.Queries.Meetings;
using Application.Queries.Tasks;
using MediatR;

namespace Application.Commands.GoogleCalendar;

public class SyncGoogleCalendarCommandHandler : BaseCommandHandler, IRequestHandler<SyncGoogleCalendarCommand>
{
    private readonly IGoogleCalendarService _googleCalendarService;
    private readonly IMediator _mediator;

    public SyncGoogleCalendarCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IGoogleCalendarService googleCalendarService,
        IMediator mediator)
        : base(unitOfWorkFactory)
    {
        _googleCalendarService = googleCalendarService;
        _mediator = mediator;
    }

    public async Task Handle(SyncGoogleCalendarCommand request, CancellationToken cancellationToken)
    {
        // 1. Load user and validate Google Calendar is connected
        Domain.Entities.User user;
        using (var unitOfWork = await UnitOfWorkFactory.CreateAsync(cancellationToken))
        {
            user = await unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken)
                ?? throw new NotFoundException(nameof(Domain.Entities.User), request.UserId);

            if (!user.IsGoogleCalendarConnected
                || string.IsNullOrEmpty(user.GoogleAccessToken)
                || string.IsNullOrEmpty(user.GoogleRefreshToken))
            {
                throw new ValidationException("Google Calendar is not connected for this user.");
            }
        }

        // 2. Compute the calendar window (full calendar month)
        var startDate = new DateTime(request.Month.Year, request.Month.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        // 3. Fetch calendar data via existing query handlers (respects all existing auth/filtering logic)
        var meetings = await _mediator.Send(
            new GetMeetingCalendarItemsQuery(request.UserId, startDate, endDate),
            cancellationToken);

        var tasks = await _mediator.Send(
            new GetTasksForCalendarMonthQuery(request.UserId, startDate),
            cancellationToken);

        // 4. Push to Google Calendar
        await _googleCalendarService.SyncMeetingsAsync(
            user.GoogleAccessToken,
            user.GoogleRefreshToken,
            meetings,
            cancellationToken);

        await _googleCalendarService.SyncTasksAsync(
            user.GoogleAccessToken,
            user.GoogleRefreshToken,
            tasks,
            cancellationToken);
    }
}

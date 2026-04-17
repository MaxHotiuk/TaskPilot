using Application.Abstractions.Calendar;
using Application.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Common.Handlers;
using MediatR;

namespace Application.Commands.GoogleCalendar;

public class ConnectGoogleCalendarCommandHandler : BaseCommandHandler, IRequestHandler<ConnectGoogleCalendarCommand>
{
    private readonly IGoogleCalendarService _googleCalendarService;

    public ConnectGoogleCalendarCommandHandler(
        IUnitOfWorkFactory unitOfWorkFactory,
        IGoogleCalendarService googleCalendarService)
        : base(unitOfWorkFactory)
    {
        _googleCalendarService = googleCalendarService;
    }

    public async Task Handle(ConnectGoogleCalendarCommand request, CancellationToken cancellationToken)
    {
        var tokenResult = await _googleCalendarService.ExchangeCodeForTokensAsync(
            request.Code,
            cancellationToken);

        await ExecuteInTransactionAsync(async unitOfWork =>
        {
            var user = await unitOfWork.Users.GetByIdAsync(request.UserId, cancellationToken)
                ?? throw new NotFoundException(nameof(Domain.Entities.User), request.UserId);

            user.GoogleAccessToken = tokenResult.AccessToken;
            user.GoogleRefreshToken = tokenResult.RefreshToken;
            user.GoogleTokenExpiry = tokenResult.Expiry;
            user.IsGoogleCalendarConnected = true;
            user.UpdatedAt = DateTime.UtcNow;

            unitOfWork.Users.Update(user);
        }, cancellationToken);
    }
}

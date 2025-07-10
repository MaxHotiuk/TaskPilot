using Application.Abstractions.Authentication;
using Application.Queries.Boards;
using Application.Queries.BoardMembers;
using Application.Queries.Tasks;
using Application.Queries.States;
using Microsoft.AspNetCore.Authorization;
using Domain.Common.Authorization;
using MediatR;
using System.Text.Json;

namespace WebApi.Authorization;

public class BoardMemberOrOwnerAuthorizationHandler : AuthorizationHandler<BoardMemberOrOwnerRequirement>
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IMediator _mediator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BoardMemberOrOwnerAuthorizationHandler(
        IAuthenticationService authenticationService,
        IMediator mediator,
        IHttpContextAccessor httpContextAccessor)
    {
        _authenticationService = authenticationService;
        _mediator = mediator;
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        BoardMemberOrOwnerRequirement requirement)
    {
        if (!await _authenticationService.IsUserAuthenticatedAsync())
        {
            context.Fail();
            return;
        }

        var userRole = await _authenticationService.GetCurrentUserRoleAsync();
        
        if (userRole == Roles.Admin)
        {
            context.Succeed(requirement);
            return;
        }

        var currentUserId = await _authenticationService.GetCurrentUserIdAsync();
        if (string.IsNullOrEmpty(currentUserId) || !Guid.TryParse(currentUserId, out var userId))
        {
            context.Fail();
            return;
        }

        var boardId = await GetBoardIdFromContextAsync();
        if (boardId == null)
        {
            context.Fail();
            return;
        }

        try
        {
            var board = await _mediator.Send(new GetBoardByIdQuery(boardId.Value), CancellationToken.None);
            if (board != null && board.OwnerId == userId)
            {
                context.Succeed(requirement);
                return;
            }

            var isMember = await _mediator.Send(new CheckBoardMembershipQuery(boardId.Value, userId), CancellationToken.None);
            if (isMember)
            {
                context.Succeed(requirement);
                return;
            }
        }
        catch
        {
            context.Fail();
            return;
        }

        context.Fail();
    }

    private async Task<Guid?> GetBoardIdFromContextAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
            return null;

        if (httpContext.Request.RouteValues.TryGetValue("boardId", out var boardIdValue) && 
            Guid.TryParse(boardIdValue?.ToString(), out var boardIdParsed))
        {
            return boardIdParsed;
        }

        var path = httpContext.Request.Path.Value?.ToLower();
        if (path != null && path.StartsWith("/api/boards/") && 
            httpContext.Request.RouteValues.TryGetValue("id", out var boardDirectIdValue) && 
            Guid.TryParse(boardDirectIdValue?.ToString(), out var boardDirectId))
        {
            return boardDirectId;
        }

        if (path != null && path.StartsWith("/api/tasks/") && 
            httpContext.Request.RouteValues.TryGetValue("id", out var taskIdValue) && 
            Guid.TryParse(taskIdValue?.ToString(), out var taskId))
        {
            try
            {
                var task = await _mediator.Send(new GetTaskItemByIdQuery(taskId), CancellationToken.None);
                if (task != null)
                {
                    return task.BoardId;
                }
            }
            catch
            {
            }
        }

        if (path != null && path.StartsWith("/api/states/") && 
            httpContext.Request.RouteValues.TryGetValue("id", out var stateIdValue) && 
            int.TryParse(stateIdValue?.ToString(), out var stateId))
        {
            try
            {
                var state = await _mediator.Send(new GetStateByIdQuery(stateId), CancellationToken.None);
                if (state != null)
                {
                    return state.BoardId;
                }
            }
            catch
            {
            }
        }

        if (path != null && path.Equals("/api/tasks") &&
            httpContext.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                httpContext.Request.EnableBuffering();

                httpContext.Request.Body.Position = 0;

                using var reader = new StreamReader(httpContext.Request.Body, leaveOpen: true);
                var body = await reader.ReadToEndAsync();

                httpContext.Request.Body.Position = 0;

                if (!string.IsNullOrEmpty(body))
                {
                    using var document = JsonDocument.Parse(body);
                    if (document.RootElement.TryGetProperty("boardId", out var boardIdProperty) &&
                        boardIdProperty.TryGetGuid(out var boardIdFromBody))
                    {
                        return boardIdFromBody;
                    }
                }
            }
            catch
            {
            }
        }

        if (httpContext.Request.Query.TryGetValue("boardId", out var queryBoardId) && 
            Guid.TryParse(queryBoardId, out var queryBoardIdParsed))
        {
            return queryBoardIdParsed;
        }

        return null;
    }
}

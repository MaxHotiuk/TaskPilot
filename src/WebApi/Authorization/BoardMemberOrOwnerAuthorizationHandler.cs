using Application.Abstractions.Authentication;
using Application.Queries.Boards;
using Application.Queries.BoardMembers;
using Application.Queries.Tasks;
using Microsoft.AspNetCore.Authorization;
using Domain.Common.Authorization;
using MediatR;

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

        if (httpContext.Request.RouteValues.TryGetValue("id", out var idValue) && 
            Guid.TryParse(idValue?.ToString(), out var boardId))
        {
            return boardId;
        }

        if (httpContext.Request.RouteValues.TryGetValue("boardId", out var boardIdValue) && 
            Guid.TryParse(boardIdValue?.ToString(), out var boardIdParsed))
        {
            return boardIdParsed;
        }

        if (httpContext.Request.RouteValues.TryGetValue("id", out var taskIdValue) && 
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

        if (httpContext.Request.Query.TryGetValue("boardId", out var queryBoardId) && 
            Guid.TryParse(queryBoardId, out var queryBoardIdParsed))
        {
            return queryBoardIdParsed;
        }

        return null;
    }
}

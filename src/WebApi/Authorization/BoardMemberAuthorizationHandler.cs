using Application.Abstractions.Authentication;
using Application.Queries.Boards;
using Application.Queries.BoardMembers;
using Application.Queries.Tasks;
using Microsoft.AspNetCore.Authorization;
using Domain.Common.Authorization;
using Microsoft.Extensions.Logging;
using MediatR;

namespace WebApi.Authorization;

public class BoardMemberAuthorizationHandler : AuthorizationHandler<BoardMemberRequirement>
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IMediator _mediator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<BoardMemberAuthorizationHandler> _logger;

    public BoardMemberAuthorizationHandler(
        IAuthenticationService authenticationService,
        IMediator mediator,
        IHttpContextAccessor httpContextAccessor,
        ILogger<BoardMemberAuthorizationHandler> logger)
    {
        _authenticationService = authenticationService;
        _mediator = mediator;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        BoardMemberRequirement requirement)
    {
        _logger.LogInformation("BoardMemberRequirement authorization check started");
        
        if (!await _authenticationService.IsUserAuthenticatedAsync())
        {
            _logger.LogWarning("User is not authenticated");
            context.Fail();
            return;
        }

        var userRole = await _authenticationService.GetCurrentUserRoleAsync();
        _logger.LogInformation("User role: {UserRole}", userRole);
        
        if (userRole == Roles.Admin)
        {
            _logger.LogInformation("User is admin, granting access");
            context.Succeed(requirement);
            return;
        }

        var currentUserId = await _authenticationService.GetCurrentUserIdAsync();
        _logger.LogInformation("Current user ID: {UserId}", currentUserId);
        
        if (string.IsNullOrEmpty(currentUserId) || !Guid.TryParse(currentUserId, out var userId))
        {
            _logger.LogWarning("Could not parse user ID: {UserId}", currentUserId);
            context.Fail();
            return;
        }

        var boardId = await GetBoardIdFromContextAsync();
        _logger.LogInformation("Board ID resolved: {BoardId}", boardId);
        
        if (boardId == null)
        {
            _logger.LogWarning("Could not resolve board ID from context");
            context.Fail();
            return;
        }

        try
        {
            var board = await _mediator.Send(new GetBoardByIdQuery(boardId.Value), CancellationToken.None);
            _logger.LogInformation("Board found: {BoardExists}, OwnerId: {OwnerId}", 
                board != null, board?.OwnerId);
                
            if (board != null && board.OwnerId == userId)
            {
                _logger.LogInformation("User is board owner, granting access");
                context.Succeed(requirement);
                return;
            }

            if (requirement.RequireOwner)
            {
                _logger.LogWarning("Requirement requires owner but user is not owner");
                context.Fail();
                return;
            }

            if (requirement.AllowMember)
            {
                var isMember = await _mediator.Send(new CheckBoardMembershipQuery(boardId.Value, userId), CancellationToken.None);
                _logger.LogInformation("User is board member: {IsMember}", isMember);
                
                if (isMember)
                {
                    _logger.LogInformation("User is board member, granting access");
                    context.Succeed(requirement);
                    return;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during board authorization check");
            context.Fail();
            return;
        }

        _logger.LogWarning("User does not have access to board");
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

using Application.Abstractions.Authentication;
using Application.Queries.Comments;
using Microsoft.AspNetCore.Authorization;
using Domain.Common.Authorization;
using MediatR;

namespace WebApi.Authorization;

public class CommentOwnerAuthorizationHandler : AuthorizationHandler<CommentOwnerRequirement>
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IMediator _mediator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CommentOwnerAuthorizationHandler(
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
        CommentOwnerRequirement requirement)
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

        var commentId = GetCommentIdFromContext();
        if (commentId == null)
        {
            context.Fail();
            return;
        }

        try
        {
            var comment = await _mediator.Send(new GetCommentByIdQuery(commentId.Value), CancellationToken.None);
            if (comment != null && comment.AuthorId == userId)
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

    private Guid? GetCommentIdFromContext()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
            return null;

        if (httpContext.Request.RouteValues.TryGetValue("id", out var idValue) && 
            Guid.TryParse(idValue?.ToString(), out var commentId))
        {
            return commentId;
        }

        if (httpContext.Request.RouteValues.TryGetValue("commentId", out var commentIdValue) && 
            Guid.TryParse(commentIdValue?.ToString(), out var commentIdParsed))
        {
            return commentIdParsed;
        }

        return null;
    }
}

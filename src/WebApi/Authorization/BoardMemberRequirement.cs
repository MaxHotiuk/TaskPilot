using Microsoft.AspNetCore.Authorization;

namespace WebApi.Authorization;

public class BoardMemberRequirement : IAuthorizationRequirement
{
    public bool RequireOwner { get; }
    public bool AllowMember { get; }

    public BoardMemberRequirement(bool requireOwner = false, bool allowMember = true)
    {
        RequireOwner = requireOwner;
        AllowMember = allowMember;
    }
}

public class BoardOwnerRequirement : IAuthorizationRequirement
{
}

public class BoardMemberOrOwnerRequirement : IAuthorizationRequirement
{
}

public class CommentOwnerRequirement : IAuthorizationRequirement
{
}

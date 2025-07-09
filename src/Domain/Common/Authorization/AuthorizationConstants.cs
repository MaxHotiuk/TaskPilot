namespace Domain.Common.Authorization;

public static class Roles
{
    public const string Admin = "Admin";
    public const string User = "User";
    
    public static readonly string[] All = { Admin, User };
}

public static class Policies
{
    public const string RequireAdminRole = "RequireAdminRole";
    public const string RequireUserRole = "RequireUserRole";
    public const string RequireBoardMember = "RequireBoardMember";
    public const string RequireBoardOwner = "RequireBoardOwner";
    public const string RequireBoardMemberOrOwner = "RequireBoardMemberOrOwner";
    public const string RequireCommentOwner = "RequireCommentOwner";
}

public static class BoardMemberRoles
{
    public const string Owner = "Owner";
    public const string Admin = "Admin";
    public const string Member = "Member";
    
    public static readonly string[] All = { Owner, Admin, Member };
}

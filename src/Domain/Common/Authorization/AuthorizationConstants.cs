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
}

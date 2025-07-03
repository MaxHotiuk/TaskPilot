namespace Application.Abstractions.Authentication;

public interface IAuthenticationService
{
    Task<string?> GetCurrentUserIdAsync();
    Task<string?> GetCurrentUserEmailAsync();
    Task<bool> IsUserAuthenticatedAsync();
    Task<string?> GetCurrentUserEntraIdAsync();
}

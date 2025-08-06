namespace Domain.Entities;

public class UserProfile : AuditableEntity<Guid>
{
    public Guid UserId { get; set; }

    // Basic Profile Information
    public string? Bio { get; set; }
    public string? JobTitle { get; set; }
    public string? Department { get; set; }
    public string? Location { get; set; }
    public string? PhoneNumber { get; set; }

    // Privacy Settings
    public bool AddToBoardAutomatically { get; set; } = false;
    public bool ShowEmail { get; set; } = false;
    public bool ShowPhoneNumber { get; set; } = false;
}
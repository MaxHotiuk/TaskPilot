namespace Domain.Dtos.Organizations;

public class OrganizationSummaryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty; // Guest, Member, or Manager
}

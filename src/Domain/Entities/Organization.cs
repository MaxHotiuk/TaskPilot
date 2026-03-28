namespace Domain.Entities;

public class Organization : AuditableEntity<Guid>
{
    public string Domain { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    // Navigation properties
    public ICollection<OrganizationMember> Members { get; set; } = new List<OrganizationMember>();
    public ICollection<Chat> Chats { get; set; } = new List<Chat>();
}

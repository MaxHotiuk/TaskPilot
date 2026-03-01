using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // DbSets for entities
    public DbSet<User> Users { get; set; }
    public DbSet<Board> Boards { get; set; }
    public DbSet<State> States { get; set; }
    public DbSet<TaskItem> Tasks { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<BoardMember> BoardMembers { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Backlog> Backlog { get; set; }
    public DbSet<Meeting> Meetings { get; set; }
    public DbSet<MeetingMember> MeetingMembers { get; set; }
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<OrganizationMember> OrganizationMembers { get; set; }
    public DbSet<OrganizationManagerRequest> OrganizationManagerRequests { get; set; }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<ChatMember> ChatMembers { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }
    public DbSet<BoardInvitation> BoardInvitations { get; set; }
    public DbSet<OrganizationInvitation> OrganizationInvitations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}

namespace Application.Abstractions.Persistence;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IBoardRepository Boards { get; }
    ITaskItemRepository Tasks { get; }
    ITagRepository Tags { get; }
    IStateRepository States { get; }
    IBoardMemberRepository BoardMembers { get; }
    ICommentRepository Comments { get; }
    INotificationRepository Notifications { get; }
    IBacklogRepository Backlogs { get; }
    IMeetingRepository Meetings { get; }
    IMeetingMemberRepository MeetingMembers { get; }
    IOrganizationRepository Organizations { get; }
    IOrganizationMemberRepository OrganizationMembers { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

    bool HasActiveTransaction { get; }
}
